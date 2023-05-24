$(document).ready(function () {
    

        
    $(document).on('click', '.addToBasket', (function () {
        let productId = $(this).data('id');
        console.log(productId)
        fetch('/basket/AddBasket?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {

                $('.header-cart').html(data);
                $(".basket .basket-icon").click(function () {
                    $(".basket-content").toggle("d-block");
                    $(".basket-content .close-div").click(function () {
                        $(".basket-content").css({ "display": "none" });
                        console.log("hhwllo");
                    })
                });
            })

    }))
    //DeCrease And InCrease Count
    $(document).on('click', '.plus-btn', function () {

        let productId = $(this).attr('data-id')
        console.log(productId)
        fetch("/basket/IncreaseCount?productId=" + productId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.cart-area').html(data)
            });

    })

    $(document).on('click', '.minus-btn', function () {

        let productId = $(this).attr('data-id')
        console.log(productId)
        fetch("/basket/DecreaseCount?productId=" + productId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.cart-area').html(data)
            });



    })


    $(document).on('click', '.removeProdInCart', function () {


        let productId = $(this).attr('data-id');
        console.log(productId)
        fetch("/basket/DeleteCart?id=" + productId).then(res => {
            return res.text();
        })
            .then(data => {
                $('.cart-area').html(data);
            })
    })



    //$('.addToBasket').click(function myfunction(e) {
    //    e.preventDefault();
    //    console.log("hhjjjjjjjjj");
    //    let productId = $(this).data('id');

    //    fetch('/basket/AddBasket?id=' + productId)
    //        .then(res => {
    //            return res.text();
    //        }).then(data => {

    //            $('.header-cart').html(data);
    //        })
    //})

    $(document).on('click', '.addAddress', function (e) {
        e.preventDefault();

        $('.addressContainer').addClass('d-none');
        $('.addressForm').removeClass('d-none');
    })
})

$('.slider-for').slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    arrows: false,
    fade: true,
    asNavFor: '.slider-nav'
});
$('.slider-nav').slick({
    slidesToShow: 3,
    slidesToScroll: 1,
    asNavFor: '.slider-for',
    dots: true,
    centerMode: true,
    focusOnSelect: true
});

$(document).on('click', '.secondImageDetail', (function () {
    let secondImgUrl = $(this).attr('src')
    let mainImageUrl = $('.mainImageDetail').attr('src')
    let mainBgImage = $('.mainImageDetail')
    console.log(secondImgUrl)
    console.log(mainImageUrl)
    $('.mainImageDetail').attr('src') = secondImgUrl
    $(this).attr('src') = mainImageUrl




}))