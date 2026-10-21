$(function () {
    function token() { return $('meta[name="csrf-token"]').attr('content'); }
    var pending;
    function loadCars(page) {
        if (pending) pending.abort();
        var data = $('#carFilterForm').serializeArray(); data.push({name:'Page',value:page});
        $('#filterStatus').text('Đang tải...');
        pending = $.ajax({url:'/Car/List',data:data,method:'GET'}).done(function(html){
            $('#carResults').html(html);$('#filterStatus').text('');
            var url = new URL(window.location.href);url.search = $.param(data);window.history.replaceState(null,'',url);
        }).fail(function(xhr,status){if(status!=='abort')$('#filterStatus').text('Không tải được dữ liệu. Hãy thử lại.');});
    }
    $('#carFilterForm').on('submit',function(e){e.preventDefault();loadCars(1);});
    $(document).on('click','.page-request',function(e){e.preventDefault();loadCars($(this).data('page'));});
    $(document).on('click','.btn-add-to-cart',function(){
        var button=$(this);button.prop('disabled',true);
        $.ajax({url:'/Cart/Add',method:'POST',headers:{'X-CSRF-TOKEN':token()},data:{carId:button.data('car-id')}})
        .done(function(result){$('#cart-counter').text(result.count).toggle(result.count > 0);alert('Đã lưu xe vào danh sách quan tâm.');})
        .fail(function(xhr){alert(xhr.responseJSON && (xhr.responseJSON.message || xhr.responseJSON.title) || 'Không thực hiện được thao tác. Hãy tải lại trang.');})
        .always(function(){button.prop('disabled',false);});
    });
});
