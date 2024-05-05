using eShopSolution.ViewModel.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.System.Role
{
    public class ViewRolePagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}
