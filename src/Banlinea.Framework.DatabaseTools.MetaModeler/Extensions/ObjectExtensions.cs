using Dynamitey;
using System.Linq;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasMember(this object target, string memberName) => Dynamic.GetMemberNames(target).Any(m => m.Equals(memberName));
    }
}