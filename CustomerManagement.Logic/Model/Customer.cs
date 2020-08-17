﻿using CSharpFunctionalExtensions;
using CustomerManagement.Logic.Common;
using System;
using System.Security.Cryptography.X509Certificates;

namespace CustomerManagement.Logic.Model
{
    public class Customer : Entity
    {
        private string _name;
        public virtual CustomerName Name => (CustomerName)_name;
        //public virtual CustomerName Name { get; protected set; }

        private string _primaryEmail;
        public virtual Email PrimaryEmail => (Email)_primaryEmail;

        private string _secondaryEmail;
        public virtual Maybe<Email> SecondaryEmail
        {
            get { return _secondaryEmail == null ? null : (Email)_secondaryEmail; }
            protected set { _secondaryEmail = value.Unwrap(x => x.Value); }
        }

        public virtual EmailingSettings EmailingSettings { get; protected set; }
        public virtual CustomerStatus Status { get; protected set; }

        protected Customer()
        {
        }

        public Customer(CustomerName name, Email primaryEmail, Maybe<Email> secondaryEmail, Industry industry)
            : this()
        {
            //if (name == null)
            //    throw new ArgumentNullException(nameof(name));
            //if (primaryEmail == null)
            //    throw new ArgumentNullException(nameof(primaryEmail));
            //if (industry == null)
            //    throw new ArgumentNullException(nameof(industry));

            //null guard should inject it

            _name = name;
            _primaryEmail = primaryEmail;
            SecondaryEmail = secondaryEmail;
            EmailingSettings = new EmailingSettings(industry, false);
            Status = CustomerStatus.Regular;
        }

        public virtual void DisableEmailing()
        {
            EmailingSettings = EmailingSettings.DisableEmailing();
        }

        public virtual void UpdateIndustry(Industry industry)
        {
            EmailingSettings = EmailingSettings.ChangeIndustry(industry);
        }

        public virtual bool CanBePromoted()
        {
            return Status != CustomerStatus.Gold;
        }

        public virtual void Promote()
        {
            if (!CanBePromoted())
                throw new InvalidOperationException();

            switch (Status)
            {
                case CustomerStatus.Regular:
                    Status = CustomerStatus.Preferred;
                    break;
                case CustomerStatus.Preferred:
                    Status = CustomerStatus.Gold;
                    break;
                case CustomerStatus.Gold:
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
