# Gearix – Vehicle Inventory Management System (Frontend)
<div align="center"><img width="500" height="136" alt="gear" src="https://github.com/user-attachments/assets/dad1b396-8495-4883-8b7a-9f52055da518" /></div>

**Gearix** is a comprehensive web-based Inventory Management System for vehicle parts and services. It supports three user roles – **Admin**, **Staff**, and **Customer** – each with tailored dashboards and functionalities. The frontend is built with **React**, **Tailwind CSS**, and communicates with a backend API (ASP.NET Core / Node.js).

## Features

### Admin
1. **Financial Reports** – Generate and view daily, monthly, and yearly revenue/expense/profit reports.
2. **Staff Management** – Register staff members and assign roles.
3. **Parts Management** – Add, edit, delete, and view parts with category and stock information.
4. **Purchase Invoices** – Create purchase invoices from vendors to update stock quantities.
5. **Vendor Management** – Full CRUD operations for vendors (name, email, phone, address).

### Staff
6. **Customer Registration** – Register new customers along with their vehicle details.
7. **Sales Invoices** – Sell vehicle parts and services, generate invoices (print/email).
8. **Customer Details & History** – View customer profiles, purchase history, and registered vehicles.
9. **Customer Reports** – Generate reports on regular customers, high spenders, and pending credits.
10. **Advanced Customer Search** – Search by vehicle number, phone number, customer ID, or name.
11. **Email Invoices** – Send invoices directly to customers via email.

### Customer
12. **Self‑Registration & Profile** – Customers can register, log in, and manage their profile and vehicles.
13. **Bookings & Requests** – Book service appointments, request unavailable parts, and review services.
14. **Purchase/Service History** – View all past invoices and service records.

### System‑Wide
15. **Automated Notifications** – Admin is notified of low stock (<10). Customers with unpaid credits >1 month receive automatic email reminders.
16. **Loyalty Program** – Automatic 10% discount on any single purchase exceeding Rs. 5,000.

## Tech Stack

- **React 18** – UI library
- **React Router DOM** – Client‑side routing
- **Tailwind CSS** – Styling and responsive design
- **Lucide React** – Icons
- **Axios / custom apiFetch** – HTTP requests to backend
- **Vite** – Build tool and development server
- **ESLint / Prettier** – Code quality and formatting

## Prerequisites

- **Node.js** (v18 or later) – [Download](https://nodejs.org/)
- **npm** or **bun** (v1.0 or later) – Package manager
- Backend API server running (see backend README)

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/vehicleims-frontend.git
   cd vehicleims-frontend

2. **Install dependencies:**

```bash
npm install

# or
bun install
```

3. **Environment Configuration**

Create a `.env` file in the root directory and add the backend API URL:

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

Adjust the URL to match your backend server.

4. **Start Development Server**

```bash
npm run dev

# or
bun run dev
```

The application will be available at:

```text
http://localhost:5173
```

---

5. **Build for Production**

```bash
npm run build

# or
bun run build
```

The production-ready files will be generated in the `dist` folder.

---

## Project Structure

```text
VEHICLEIMS_FRONTEND/
├── public/               # Static assets
├── src/
│   ├── api/              # API client (clientApi.js)
│   ├── assets/           # Images, fonts, etc.
│   ├── components/       # Reusable UI components
│   ├── lib/              # Utility functions
│   ├── pages/            # Page components (Admin, Staff, Customer)
│   ├── services/         # Business logic / service modules
│   ├── App.jsx           # Main app with routing
│   ├── main.jsx          # Entry point
│   └── style.css         # Global Tailwind styles
├── .env                  # Environment variables
├── package.json
├── vite.config.js
└── README.md
```

---

## Role-Based Access

### Admin
Access to:
- Admin Dashboard
- Parts Management
- Vendors
- Purchase Invoices
- Staff Management
- Financial Reports

### Staff
Access to:
- Sales
- Customer & Vehicle Registration
- Customer Search
- Reports
- Invoice Email

### Customer
Access to:
- Registration
- Profile
- Booking
- History
- Service Requests

The frontend uses **JWT tokens** returned by the backend.  
The user role is decoded from the token and used to conditionally render routes and UI elements.

---

## Available Scripts

| Command | Description |
|---|---|
| `npm run dev` | Start development server |
| `npm run build` | Create production build |
| `npm run preview` | Preview production build locally |
| `npm run lint` | Run ESLint checks |
| `npm run format` | Format code with Prettier |

---

## Key Dependencies

| Package | Version | Purpose |
|---|---|---|
| `react` | `^18.2.0` | Core UI library |
| `react-router-dom` | `^6.14.0` | Routing |
| `tailwindcss` | `^3.3.0` | Styling |
| `lucide-react` | `^0.263.0` | Icons |
| `recharts` | `^2.7.0` | Charts for financial reports |
| `html2canvas` + `jspdf` | `latest` | PDF export for reports |
| `@tanstack/react-query` | `^4.29.0` | Data fetching & caching |

---

##<img width="500" height="136" alt="gear" src="https://github.com/user-attachments/assets/7c88e8cf-0f8f-45d7-8906-7296ab409cbd" />
 Contributing

1. Fork the repository  
2. Create a feature branch:

```bash
git checkout -b feature/amazing-feature
```

3. Commit your changes:

```bash
git commit -m "Add amazing feature"
```

4. Push to the branch:

```bash
git push origin feature/amazing-feature
```

5. Open a Pull Request

---

# 📄 License

This project is for educational/personal use.  
All rights reserved.
