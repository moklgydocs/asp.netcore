using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EfCore.Domain.Shared.Enums
{
    public enum MpaaRating
    {
        G,
        PG,
        [EnumMember(Value = "PG-13")] // 映射到数据库的 "PG-13"
        PG13,
        R,
        [EnumMember(Value = "NC-17")] // 映射到数据库的 "NC-17"
        NC17
    }
}
