using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagement.Logic.Model
{
    public class EmailingSettings : ValueObject
    {
        public EmailingSettings()
        {
        }

        public EmailingSettings(Industry industry, bool emailingDisabled)
        {
            Industry = industry;
            EmailingDisabled = emailingDisabled;
        }

        public Industry Industry { get; }
        public bool EmailingDisabled { get; }
        public EmailCampaign EmailCampaign => GetEmailCampaign(Industry);

        private EmailCampaign GetEmailCampaign(Industry industry)
        {
            if (EmailingDisabled)
                return EmailCampaign.None;

            if (industry == Industry.Cars)
                return EmailCampaign.LatestCarModels;

            if (industry == Industry.Pharmacy)
                return EmailCampaign.PharmacyNews;

            if (industry == Industry.Other)
                return EmailCampaign.Generic;

            throw new ArgumentException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Industry;
            yield return EmailCampaign;
        }

        internal EmailingSettings DisableEmailing()
        {
            return new EmailingSettings(Industry, true);
        }

        internal EmailingSettings ChangeIndustry(Industry industry)
        {
            return new EmailingSettings(industry, EmailingDisabled);
        }
    }
}
