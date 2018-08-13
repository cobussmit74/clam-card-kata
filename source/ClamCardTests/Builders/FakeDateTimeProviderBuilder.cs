using ClamCard;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClamCardTests.Builders
{
    public class FakeDateTimeProviderBuilder
    {
        private FakeDateTimeProviderBuilder()
        {
            _now = DateTime.Now;
        }

        private DateTime _now;

        public static FakeDateTimeProviderBuilder Create()
        {
            return new FakeDateTimeProviderBuilder();
        }

        public static IDateTimeProvider BuildDefault()
        {
            return Create().Build();
        }

        public static IDateTimeProvider BuildFor(DateTime value)
        {
            return Create().WithNow(value).Build();
        }

        public static IDateTimeProvider BuildFor(int year, int month, int day)
        {
            return Create().WithNow(year, month, day).Build();
        }

        public FakeDateTimeProviderBuilder WithNow(DateTime value)
        {
            _now = value;
            return this;
        }

        public FakeDateTimeProviderBuilder WithNow(int year, int month, int day)
        {
            _now = new DateTime(year, month, day);
            return this;
        }

        public IDateTimeProvider Build()
        {
            var provider = Substitute.For<IDateTimeProvider>();

            provider.Now.Returns(_now);

            return provider;
        }
    }
}
