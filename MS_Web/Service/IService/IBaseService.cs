using MS_Web.Models;
using MS_Web.Models.DTO;

namespace MS_Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?>SendAsync(RequestDto requestDto, bool withBeare=true);
    }
}
