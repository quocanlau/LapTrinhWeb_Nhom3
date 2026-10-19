using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
namespace Autora.Services;
public class DatabaseErrorFilter : IExceptionFilter
{
    public void OnException(ExceptionContext c)
    {
        string? message=c.Exception switch {
            BusinessException e=>e.Message,
            DbUpdateConcurrencyException=>"Dữ liệu vừa được người khác thay đổi. Hãy tải lại và kiểm tra trạng thái xe trước khi thử lại.",
            DbUpdateException e when e.InnerException is SqlException s && s.Number is 2601 or 2627=>"Dữ liệu bị trùng hoặc xe đã được giữ bởi giao dịch khác.",
            DbUpdateException e when e.InnerException is SqlException s && s.Number==547=>"Thao tác vi phạm ràng buộc dữ liệu hoặc dữ liệu đã được sử dụng.",
            SqlException e when e.Number==1205=>"Có thao tác đồng thời. Hãy tải lại dữ liệu trước khi thử lại.",
            _=>null};
        if(message is null) return;
        c.ExceptionHandled=true;
        if(c.HttpContext.Request.Path.StartsWithSegments("/api"))
            c.Result=new ObjectResult(new ProblemDetails{Title=message,Status=409}){StatusCode=409};
        else c.Result=new ViewResult{ViewName="~/Views/Shared/OperationError.cshtml",ViewData=new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),c.ModelState){Model=message},StatusCode=409};
    }
}
