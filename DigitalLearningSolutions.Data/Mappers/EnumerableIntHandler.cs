namespace DigitalLearningSolutions.Data.Mappers
{
    using System.Data;
    using System.Linq;
    using System.Collections.Generic;
    using Dapper;

    public class EnumerableIntHandler : SqlMapper.TypeHandler<IEnumerable<int>>
    {
        public override void SetValue(IDbDataParameter parameter, IEnumerable<int> value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = string.Join(",", value);
        }

        public override IEnumerable<int> Parse(object value)
        {
            return value.ToString()?.Split(',').Select(int.Parse) ?? new List<int>();
        }
    }
}
