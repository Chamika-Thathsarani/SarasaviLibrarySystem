Sarasavi Library Management System

A desktop-based Library Management System developed using C# Windows Forms (.NET Framework) and Microsoft SQL Server. The system automates the daily operations of Sarasavi Library, including user registration, book management, loan processing, return handling, reservations, and book inquiries.

Features

User Management

Register new library members and visitors,
Update and delete user records,
Auto-generate user numbers,
NIC validation and duplicate checking

Book Management

Register new books and copies,
Auto-generate book numbers based on classification,
Support up to 10 copies per book,
Mark books as Borrowable or Reference Only,
Update book information

Loan Management

Borrow books for a 14-day period,
Maximum 5 books per member,
Prevent borrowing when overdue books exist,
Restrict visitors from borrowing books,
Prevent borrowing of reference-only books

Return Management

Process returned books,
Update book availability automatically,
Check reservations during returns,
Notify reserved users when books become available,

Reservation Management

Reserve unavailable books,
Maximum 3 reservations per member,
Prevent duplicate reservations,
Reservation queue based on oldest reservation date

Inquiry System

Search books by:
Book Number,
Title,
Author

View:
Total Copies,
Available Copies,
Borrowed Copies,
Reserved Copies,
Reference Status

Authentication
Secure administrator login,
Database-based credential validation

Technologies Used
C#,
Windows Forms (WinForms),
.NET Framework,
Microsoft SQL Server,
ADO.NET,
Object-Oriented Programming (OOP),
Visual Studio,
Database Structure,

The system uses the following tables:

Admins
Users
Books
Copies
Loans
Reservations

Business Rules Implemented
Members can borrow a maximum of 5 books,
Loan period is 14 days,
Visitors cannot borrow or reserve books,
Reference books cannot be borrowed,
Maximum 10 copies can be registered per book,
Maximum 3 reservations per member,
Users with overdue books cannot borrow or reserve books,

Screens Included

Login Form,
Dashboard,
User Management,
Book Management,
Loan Books,
Return Books,
Book Reservations,
Book Inquiry/Search,

Learning Outcomes

This project demonstrates:

Desktop Application Development,
Database Design and Management,
CRUD Operations,
SQL Server Integration,
Object-Oriented Programming,
Data Validation,
Event-Driven Programming,
Library Business Process Automation

Developed as an academic software engineering project using Visual Studio, C#, and SQL Server.
