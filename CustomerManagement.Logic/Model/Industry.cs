using CSharpFunctionalExtensions;
using CustomerManagement.Logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerManagement.Logic.Model
{
    public class Industry : Entity
    {
        public static readonly Industry Cars = new Industry(1, "Cars");
        public static readonly Industry Pharmacy = new Industry(2, "Pharmacy");
        public static readonly Industry Other = new Industry(3, "Other");

        public static readonly IReadOnlyList<Industry> All =
            new List<Industry> { Cars, Pharmacy, Other };

        protected Industry()
        {
        }

        protected Industry(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public virtual string Name { get; set; }

        public static Result<Industry> Get(Maybe<string> name)
        {
            if (name.HasNoValue)
                return Result.Failure<Industry>("Industry name is not specified");

            Maybe<Industry> industry = All.SingleOrDefault(x => x.Name == name);

            if (industry.HasNoValue)
                return Result.Failure<Industry>("Industry name is invalid" + name);

            return Result.Success(industry.Value);
        }
    }
}
