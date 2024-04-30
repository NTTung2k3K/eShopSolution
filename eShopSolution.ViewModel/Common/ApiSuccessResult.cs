﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Common
{
    public class ApiSuccessResult<T> : ApiResult<T>
    {
        public ApiSuccessResult(T resultObj) 
        {
            IsSuccessed = true;
            ResultObj = resultObj;
        }
        public ApiSuccessResult() 
        {
            IsSuccessed = true;
        }
        public ApiSuccessResult(string message)
        {
            Message = message;
            IsSuccessed = true;
        }
    }
}
