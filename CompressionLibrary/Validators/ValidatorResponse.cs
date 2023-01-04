using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionLibrary.Validators
{
    internal struct ValidatorResponse 
    {
        internal ResponseType Response { get; private set; } = ResponseType.Valid;

        internal ValidatorResponse(ResponseType responseType)
        {
            SetResponse(responseType);
        }

        internal ValidatorResponse SetResponse(ResponseType responseType)
        {
            Response = responseType;
            return this;
        }

        //internal bool IsResponseValid()
        //{
        //    var result = true;
        //    foreach (var response in responses)
        //    {
        //        if (response != ResponseType.Valid)
        //        {
        //            result = false;
        //            break;
        //        }
        //    }
        //    return result;
        //}


    }
}
