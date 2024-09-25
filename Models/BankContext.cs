using BankOfHogwarts.Models;
using Microsoft.EntityFrameworkCore;


namespace BankOfHogwarts.Models
{
    public class BankContext : DbContext
    {
        IConfiguration appconfig;
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LoanOptions> LoanOptions { get; set; }

        public BankContext(IConfiguration configuration)
        {
            appconfig = configuration;
        }

        // Constructor for testing, which uses DbContextOptions
        public BankContext(DbContextOptions<BankContext> options,bool value) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (appconfig != null && !optionsBuilder.IsConfigured)

            {
                optionsBuilder.UseSqlServer(appconfig.GetConnectionString("BankConStr"));
            }
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(appconfig.GetConnectionString("BankConStr"));
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique Constraints
            //Admin Table
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email).IsUnique();

            //Customer Table
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.ContactNumber).IsUnique();
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AadharNumber).IsUnique();
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Pan).IsUnique();

            //Employee Table
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.PhoneNumber).IsUnique();
            modelBuilder.Entity<Employee>()
                .Property(e => e.Position)
                .HasConversion<string>();


            //Branch Table
            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.IFSCCode).IsUnique();

            //Account Table
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.AccountNumber).IsUnique();
            modelBuilder.Entity<Account>()
                .Property(e => e.Status)
                .HasConversion<string>();

            // Relationships Configuration
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Assuming cascade delete for customer deletion

            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountType)
                .WithMany()
                .HasForeignKey(a => a.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of account types with existing accounts

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Branch)
                .WithMany()
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of branches with existing accounts

            modelBuilder.Entity<Account>()
               .Property(a => a.Balance)
               .HasColumnType("decimal(18, 2)"); // 18 total digits with 2 after the decimal point

            //Beneficiary Table
            modelBuilder.Entity<Beneficiary>()
                .HasOne(b => b.Branch)
                .WithMany()
                .HasForeignKey(b => b.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Beneficiary>()
                .HasOne(b => b.Account)
                .WithMany(a => a.Beneficiaries)
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // Beneficiary is deleted when the Account is deleted

            //Transaction Table
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            //Loan Table
            modelBuilder.Entity<Loan>()
                .HasOne(la => la.Account)
                .WithMany(a => a.Loans)
                .HasForeignKey(la => la.AccountId)
                .OnDelete(DeleteBehavior.Cascade); 
            modelBuilder.Entity<Loan>()
                .Property(e => e.LoanApplicationStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Loan>()
                .Property(e => e.LoanStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Loan>()
                .HasOne(la => la.LoanOptions)
                .WithMany()
                .HasForeignKey(la => la.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(la => la.Employee)
                .WithMany()
                .HasForeignKey(la => la.EmployeeId)
                .IsRequired(false); // Employee who reviewed the loan can be optional
          

            // LoanOptions 
            modelBuilder.Entity<LoanOptions>()
                .Property(lo => lo.LoanAmount)
                .HasColumnType("decimal(18, 2)"); // 18 total digits with 2 after the decimal point

            modelBuilder.Entity<LoanOptions>()
                .Property(lo => lo.InterestRate)
                .HasColumnType("decimal(5, 2)"); // 5 total digits with 2 after the decimal point
            modelBuilder.Entity<LoanOptions>()
                           .Property(e => e.LoanType)
                           .HasConversion<string>();


            base.OnModelCreating(modelBuilder);
        }
    }
}
