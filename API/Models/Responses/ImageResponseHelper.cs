// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace API.Models.Responses
// {
//     public static class ResponseHelper<T>
//     {
//         public static PagedResponse<T> GetImageResponse(string url, IEnumerable<T> result, int pageNumber, int pageSize, int totalImages)
//         {
//             var response = new PagedResponse<T>(result);
//             var totalpages = Math.Ceiling(totalImages / (double)pageSize);
//             response.Meta.Add("totalImages", totalImages.ToString());
//             response.Meta.Add("TotalPages", totalpages.ToString());

//             response.Links.Add("next", pageNumber == totalpages ? "" : $"{url}?pageNumber={pageNumber + 1}");
//             response.Links.Add("prev", pageNumber == 1 ? "" : $"{url}?pageNumber={pageNumber - 1}");
//             response.Links.Add("first", $"{url}?pageNumber=1");
//             response.Links.Add("last", $"{url}?pageNumber={totalpages}");
//             return response;
//         }
//     }
// }