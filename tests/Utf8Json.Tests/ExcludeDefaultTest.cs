using Xunit;

namespace Utf8Json.Tests
{
    public class ExcludeDefaultTest
    {
        [Fact]
        public void TestFilledValues()
        {
            var person = new SimplePerson()
            {
                Age = 18,
                Name = "Bob",
                Number = 3147483647,
                IsHuman = true,
                Probability = 0.7891m,
                Size = 178,
                Color = Color.Blue
            };

            var answer = "{\"Age\":18,\"Name\":\"Bob\",\"Number\":3147483647,\"IsHuman\":true,\"Probability\":0.7891,\"Size\":178,\"Color\":\"Blue\"}";
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.Default).Is(answer);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.ExcludeNull).Is(answer);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.ExcludeNullExcludeDefault).Is(answer);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivate).Is(answer);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(answer);
        }

        [Fact]
        public void TestDefaultValues()
        {
            var person = new SimplePerson();

            var answer1 = "{\"Age\":0,\"Name\":null,\"Number\":0,\"IsHuman\":false,\"Probability\":0,\"Size\":0,\"Color\":\"None\"}";
            var answer2 = "{\"Age\":0,\"Number\":0,\"IsHuman\":false,\"Probability\":0,\"Size\":0,\"Color\":\"None\"}";
            var answerDefault = "{}";
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.Default).Is(answer1);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.ExcludeNull).Is(answer2);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.ExcludeNullExcludeDefault).Is(answerDefault);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivate).Is(answer1);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(answerDefault);
        }

        [Fact]
        public void TestNullValues()
        {
            var person = new IncludeNullablePerson();

            var answer3 = "{\"Age\":null,\"Name\":null}";
            var answer4 = "{}";
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.Default).Is(answer3);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.ExcludeNull).Is(answer4);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.ExcludeNullExcludeDefault).Is(answer4);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivate).Is(answer3);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivateExcludeNull).Is(answer4);
            JsonSerializer.ToJsonString(person, Utf8Json.Resolvers.StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(answer4);
        }

        public class SimplePerson
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public long Number { get; set; }
            public bool IsHuman { get; set; }
            public decimal Probability { get; set; }
            public double Size { get; set; }
            public Color Color { get; set; }
        }

        public class IncludeNullablePerson
        {
            public int? Age { get; set; }
            public string Name { get; set; }
        }

        public enum Color
        {
            None = 0,
            Blue = 1
        }
    }
}