using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using VehicleIMS.Application.DTOs;
using VehicleIMS.Application.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace VehicleIMS.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendInvoiceEmailAsync(InvoiceSummaryDTO invoice, string recipientEmail)
    {
        var emailSection = _config.GetSection("Email");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            emailSection["SenderName"] ?? "VehicleIMS",
            emailSection["SenderEmail"]));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = $"Invoice {invoice.InvoiceNumber} — Vehicle IMS";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = BuildHtmlInvoice(invoice)
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();

        var host = emailSection["SmtpHost"] ?? throw new InvalidOperationException("SMTP host not configured.");
        var port = int.Parse(emailSection["SmtpPort"] ?? "587");
        var useSsl = bool.Parse(emailSection["UseSsl"] ?? "false");

        await smtp.ConnectAsync(host, port,
            useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            emailSection["SenderEmail"],
            emailSection["Password"]);

        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("Invoice {Invoice} emailed to {Email}",
            invoice.InvoiceNumber, recipientEmail);
    }

    // ── HTML template ────────────────────────────────────────────────────────

    private static string BuildHtmlInvoice(InvoiceSummaryDTO inv)
    {
        var rows = string.Join("", inv.Items.Select(i => $@"
            <tr>
              <td style=""padding:8px;border-bottom:1px solid #eee"">{i.PartName}</td>
              <td style=""padding:8px;border-bottom:1px solid #eee;text-align:center"">{i.Quantity}</td>
              <td style=""padding:8px;border-bottom:1px solid #eee;text-align:right"">Rs.&nbsp;{i.UnitPrice:N2}</td>
              <td style=""padding:8px;border-bottom:1px solid #eee;text-align:right"">Rs.&nbsp;{i.LineTotal:N2}</td>
            </tr>
        "));

        var serviceRow = inv.ServiceInfo is null ? "" : $@"
            <tr>
              <td colspan=""3"" style=""padding:8px;border-bottom:1px solid #eee"">Service: {inv.ServiceInfo}</td>
              <td style=""padding:8px;border-bottom:1px solid #eee;text-align:right"">Rs.&nbsp;{inv.ServiceCharge:N2}</td>
            </tr>
        ";

        var vehicleHtml = inv.VehicleInfo is null ? "" : $"<p><strong>Vehicle:</strong> {inv.VehicleInfo}</p>";

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
          <meta charset=""utf-8""/>
          <style>
            body {{font-family: Arial, sans-serif; color: #333; margin: 0; padding: 0; }}
            .wrapper {{max-width: 640px; margin: 24px auto; background: #fff; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; }}
            .header {{background: #1a56db; color: #fff; padding: 24px 32px; }}
            .header h1 {{margin: 0; font-size: 22px; }}
            .header p  {{margin: 4px 0 0; opacity: .8; font-size: 13px; }}
            .body {{padding: 32px; }}
            table {{width: 100%; border-collapse: collapse; margin-top: 16px; }}
            th {{background: #f5f7ff; text-align: left; padding: 10px 8px; font-size: 12px; text-transform: uppercase; letter-spacing: .05em; }}
            .summary-row td {{padding: 6px 8px; }}
            .total-row td {{padding: 10px 8px; font-weight: bold; font-size: 16px; border-top: 2px solid #1a56db; }}
            .footer {{background: #f9fafb; padding: 16px 32px; font-size: 12px; color: #888; text-align: center; }}
          </style>
        </head>
        <body>
          <div class=""wrapper"">
            <div class=""header"">
              <h1>Vehicle IMS</h1>
              <p>Tax Invoice / Receipt</p>
            </div>
            <div class=""body"">
              <table style=""margin-bottom:24px;border:none"">
                <tr>
                  <td style=""width:50%;vertical-align:top;border:none;padding:0"">
                    <strong>Bill To</strong><br/>
                    {inv.CustomerName}<br/>
                    {inv.CustomerEmail}<br/>
                    {inv.CustomerPhone}
                  </td>
                  <td style=""width:50%;vertical-align:top;text-align:right;border:none;padding:0"">
                    <strong>Invoice #</strong> {inv.InvoiceNumber}<br/>
                    <strong>Date:</strong> {inv.SalesDate:dd MMM yyyy}<br/>
                    <strong>Payment:</strong> {inv.PaymentMethod}<br/>
                    <strong>Status:</strong> {inv.PaymentStatus}
                  </td>
                </tr>
              </table>

              {vehicleHtml}

              <table>
                <thead>
                  <tr>
                    <th>Description</th>
                    <th style=""text-align:center"">Qty</th>
                    <th style=""text-align:right"">Unit Price</th>
                    <th style=""text-align:right"">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {rows}
                  {serviceRow}
                </tbody>
                <tfoot>
                  <tr class=""summary-row"">
                    <td colspan=""3"" style=""text-align:right;padding:8px"">Parts Sub-total</td>
                    <td style=""text-align:right;padding:8px"">Rs.&nbsp;{inv.PartsTotal:N2}</td>
                  </tr>
                  <tr class=""summary-row"">
                    <td colspan=""3"" style=""text-align:right;padding:8px"">Service Charge</td>
                    <td style=""text-align:right;padding:8px"">Rs.&nbsp;{inv.ServiceCharge:N2}</td>
                  </tr>
                  <tr class=""summary-row"">
                    <td colspan=""3"" style=""text-align:right;padding:8px"">Loyalty Discount (10%)</td>
                    <td style=""text-align:right;padding:8px;color:#e53e3e"">- Rs.&nbsp;{inv.Discount:N2}</td>
                  </tr>
                  <tr class=""total-row"">
                    <td colspan=""3"" style=""text-align:right"">Total</td>
                    <td style=""text-align:right"">Rs.&nbsp;{inv.Total:N2}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
            <div class=""footer"">
              Thank you for your business! This is a Gearix VehicleIMS Invoice.
            </div>
          </div>
        </body>
        </html>
        ";
    }
}