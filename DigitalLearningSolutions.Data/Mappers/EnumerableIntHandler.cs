using System.Collections.Generic;

namespace DigitalLearningSolutions.Data.Mappers
{
    using System.Data;
    using System.Linq;
    using Dapper;

    public class EnumerableIntHandler : SqlMapper.TypeHandler<IEnumerable<int>>
    {
        public override void SetValue(IDbDataParameter parameter, IEnumerable<int> value)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<int> Parse(object value)
        {
            return value.ToString()?.Split(',').Select(int.Parse) ?? new List<int>();
        }
    }
}
