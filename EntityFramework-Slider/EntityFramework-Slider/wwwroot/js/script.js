$(document).ready(function () {
    //LOAD-MORE
    $(document).on("click", ".load-more", function () {

        let parent = $(".parent-products");
        //skipCount=htmlde olan clasin childrenlerin sayi,lazimdi her appen eliyende negeder skip=yani  buraxaq deye
        let skipCount = $(parent).children().length;

        let dataCount = $(parent).attr("data-count");
        //ajax-vasitesi ile request atiriq Urlere
        $.ajax({

            //urlde:-contoleri ve actionu yaziriq ve skip-adli varebla deyer gonderik,ordan gebul edib skip elesin deye
            url: `shop/loadmore?skip=${skipCount}`,
            //type:-datani gotururuk deye type=get
            type: "Get",

            //succsesden sonra hansi function islesin
            success: function (res) { 

                $(parent).append(res);

                //skipCount-burada cagirirsan cunku yuxarda caqirib methoda gonderirik,ama appenden sonra reqem deyisir deye bura yeniden cagirmali oluruq
                skipCount = $(parent).children().length;
                if (skipCount >= dataCount) {
                    $(".load-more").addClass("d-none")
                    $(".show-less").removeClass("d-none")
                }
            }
        })



    })
    //--------SHOW-LESS------------
    $(document).on("click", ".show-less", function () {

       //show-lesse-basanda butun productlar itsin esas olanlar gorsensin
        let skipCount = 0;

        
        $.ajax({

            url: `shop/loadmore?skip=${skipCount}`,

            type: "Get",

        
            success: function (res) {

                //html-""-bos edirikki kohnenin ustune yazdirmayaq esaslari
                $(".parent-products").html("");
                $(".parent-products").append(res);
               
                    $(".load-more").removeClass("d-none")
                    $(".show-less").addClass("d-none")
                
            }
        })

    })


    //------------SEACRCH PRODUCT-----------

    $(document).on("keyup", "#input-search", function () {   //search inputun keyupinda(elvi qaldiranda) islesin

        $("#search-list li").slice(1).remove();       //
        let value = $("#input-search").val();      //inputun valusun elde edirik
        $.ajax({    //request getsin methoda

            url: "shop/search?searchText="+value,    //bu Urle getsin request ve inputun deyerini Actiona arqument kimi gonderiririk

            type: "Get",

            success: function (res) {

                $("#search-list").append(res);   //Ul-eye append edirik res(method bize PartilaView qaytarir)
            }



        })

    })

    //---------------ADD-PRODUCTS TO BASKET WITH REQUEST AJAX(Ajaxla productu add ediremki refresh olmasin)---------------

    $(document).on("click", ".add-basket", function () {    //add-basket butona basdiqda islesin

        let productId = $(this).parent().attr("data-id")   //hemin add-basket butonuna aid olan productun Id-sini goturuk

        let data = { id: productId }  //objexte beraber edirik bucur gonderik actiona productun Id-sini(bezpraktis).
        $.ajax({

            url: "home/addBasket",     //add-basket Action request atiriq
            type: "Post",
            data: data,
            success: function (res) {           
                $(".card-count").text(res);  //actiondan butun productlari countlarinin Sumi gelir
               swal("Product added to basket", "", "success");  //sweet-alert

            }
        })
        return false;


    })


    //---------------DELETE PRODUCT FROM BASKET WITH AJAX------------------------

    $(document).on("click", ".delete-basketProduct", function () {      //delete butona basdiqda islesin

        

        let deletProduct = $(this).parent().parent();                  //hemin delete edeceymiz productun row-nu goturuk
       
        let productId = $(this).attr("data-id")                         //hemin delete butonuna aid olan productun Id-sini goturuk
    
        let sum = 0;
      
        let grandTotal = $(".total-product").children().eq(0);          //umumi butun productlarin giymetin goturuk(deyismek ucun sildikden sonra)

  
        $.ajax({

            url: `card/DeleteProductFromBasket?id=${productId}`,       //productu basketden silen Actiona request atiriq

            type: "Post",                                              
            success: function (res) {
                res--
                $(".card-count").text(res)
                $(deletProduct).remove();                              //siline productun row-nu silirik
                swal("Product deleted to basket", "", "success");
              
                for (const product of $(".table-product").children()) {     //tabledaki-rowlari(productlari) fora saliriiq
                    let total = parseFloat($(product).children().eq(6).text())   //butun productlarin ara-totalini goturuk
                    sum += total    //ara totallari toplayiriq
                 
                }
                $(grandTotal).text(sum);   //ve umimi totala (ara totallarin cemini) ekleyirik


              

                if ($(".table-product").children().length == 0) {     //eyer tablda-row(product) yoxdusa 
                    $("table").addClass("d-none");                      //hemin table d-none et
                    $(".total-product").addClass("d-none");             //grand-totalida d-none edirik
                    $(".alert-product").removeClass("d-none");            //alertden d-noni silirik
                 
                  
                }
                 
              
                    
            }
        })
        return false;


    })




    //Decrease Product from Basket (product sayini azaltmaq basketde) 
    $(document).on("click", ".minus", function () {


        let productId = $(this).parent().parent().attr("data-id");   //hemin minusa aid olan productun Id-sini gotururuk

        let input = $(this).next()     //inputu goturuk

        let count = parseInt($(input).val()) - 1;    //ve inputun icindeki deyeri bidefe azaldiriq


        let nativePrice = parseFloat($(this).parent().prev().text())   //productun ilkin qiymetin gotururuk

        let total = $(this).parent().next().children().eq(0);          //bir productun umumi totalin goturuk

        let sum = 0;

        let grandTotal = $(".total-product").children().eq(0);          //butun productlari toplam giymetin goturuk deyismek ucun


        if (count > 0) {      //ve eyer count 1 den-yuxaridisa edirik  1dende az - azaltmaq olmasin deye inputun deyerini(productun sayini)      
            $(input).val(count);  

                $.ajax({

                    url: `card/DecreaseCountProductFromBasket?id=${productId}`,      //productu countunu azaldan actiona request atiriq

                    type: "Post",

                    success: function (res) {
                        
                      let countProduct =res;              //actiondan gelen azalmis productun sayi
                      let subtotal = nativePrice * countProduct  //subtotal = productun ilkin giymetiynen indiki sayini vururuq
                      total.text(subtotal + ",00")              //productun ara-totalina yazdiriq subtotali
                      for (const product of $(".table-product").children()) {     //tablda olan rowlari(productlari) fora saliriq
                   
                       let total = parseFloat($(product).children().eq(6).text())   //butun productlarin ara-totallarini gotururuk
                       sum += total   //ve ara-totallari toplayiriq
                   

                      }
                        $(grandTotal).text(sum + ",00");   //topladiqimiz ara-totallari yazdiriq grandTotala
               
             
               
                    }
                })
        }


      

    


    })



    //Increase Product from Basket (product sayini coxaltmaq basketde) 
    $(document).on("click", ".plus", function () {

        let productId = $(this).parent().parent().attr("data-id");   //hemin plusa aid olan productun Id-sini gotururuk

        let input = $(this).prev()  //inputu goturuk

        let count = parseInt($(input).val()) + 1;   //ve inputun icindeki deyeri bidefe coxaldiriq
        
            $(input).val(count);   //ve coxaltdiqimiz deyeri  inputun valusuna elave edirik
      
        let nativePrice = parseFloat($(this).parent().prev().text())   //productun ilkin qiymetin gotururuk


        let total = $(this).parent().next().children().eq(0);                //bir productun ara-totalin goturuk

        let sum = 0;

        let grandTotal = $(".total-product").children().eq(0);      //butun productlarin toplam giymetin goturuk deyismek ucun
       

        $.ajax({

            url: `card/IncreaseCountProductFromBasket?id=${productId}`,         //productu countunu coxaldan actiona request atiriq

            type: "Post",
            success: function (res) {

                let countProduct = res;      //actiondan gelen coxalmis productun sayi
                let subtotal = nativePrice * countProduct    //subtotal = productun ilkin giymetiynen indiki sayini vururuq
                total.text(subtotal + ",00")                   //productun ara-totalina yazdiriq subtotali
                for (const product of $(".table-product").children()) {     //tablda olan rowlari(productlari) fora saliriq

                    let total = parseFloat($(product).children().eq(6).text())    //butun productlarin ara-totallarini gotururuk 

                    sum += total   //ve ara-totallari toplayiriq 
                  
                }
                $(grandTotal).text(sum + ",00");   //topladiqimiz ara-totallari yazdiriq grandTotala
            
                
            }
        })
        return false;

    })

   

   





    $.ajax({
        url: `card/index`,

        type: "Get",

        success: function (res) {

            

        }

        })


    // HEADER

    $(document).on('click', '#search', function () {
        $(this).next().toggle();
    })

    $(document).on('click', '#mobile-navbar-close', function () {
        $(this).parent().removeClass("active");

    })
    $(document).on('click', '#mobile-navbar-show', function () {
        $('.mobile-navbar').addClass("active");

    })

    $(document).on('click', '.mobile-navbar ul li a', function () {
        if ($(this).children('i').hasClass('fa-caret-right')) {
            $(this).children('i').removeClass('fa-caret-right').addClass('fa-sort-down')
        }
        else {
            $(this).children('i').removeClass('fa-sort-down').addClass('fa-caret-right')
        }
        $(this).parent().next().slideToggle();
    })

    // SLIDER

    $(document).ready(function(){
        $(".slider").owlCarousel(
            {
                items: 1,
                loop: true,
                autoplay: true
            }
        );
      });

    // PRODUCT

    $(document).on('click', '.categories', function(e)
    {
        e.preventDefault();
        $(this).next().next().slideToggle();
    })

    $(document).on('click', '.category li a', function (e) {
        e.preventDefault();
        let category = $(this).attr('data-id');
        let products = $('.product-item');
        
        products.each(function () {
            if(category == $(this).attr('data-id'))
            {
                $(this).parent().fadeIn();
            }
            else
            {
                $(this).parent().hide();
            }
        })
        if(category == 'all')
        {
            products.parent().fadeIn();
        }
    })

    // ACCORDION 

    $(document).on('click', '.question', function()
    {   
       $(this).siblings('.question').children('i').removeClass('fa-minus').addClass('fa-plus');
       $(this).siblings('.answer').not($(this).next()).slideUp();
       $(this).children('i').toggleClass('fa-plus').toggleClass('fa-minus');
       $(this).next().slideToggle();
       $(this).siblings('.active').removeClass('active');
       $(this).toggleClass('active');
    })

    // TAB

    $(document).on('click', 'ul li', function()
    {   
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().next().children('p.active').removeClass('active');

        $(this).parent().next().children('p').each(function()
        {
            if(dataId == $(this).attr('data-id'))
            {
                $(this).addClass('active')
            }
        })
    })

    $(document).on('click', '.tab4 ul li', function()
    {   
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().parent().next().children().children('p.active').removeClass('active');

        $(this).parent().parent().next().children().children('p').each(function()
        {
            if(dataId == $(this).attr('data-id'))
            {
                $(this).addClass('active')
            }
        })
    })

    // INSTAGRAM

    $(document).ready(function(){
        $(".instagram").owlCarousel(
            {
                items: 4,
                loop: true,
                autoplay: true,
                responsive:{
                    0:{
                        items:1
                    },
                    576:{
                        items:2
                    },
                    768:{
                        items:3
                    },
                    992:{
                        items:4
                    }
                }
            }
        );
      });

      $(document).ready(function(){
        $(".say").owlCarousel(
            {
                items: 1,
                loop: true,
                autoplay: true
            }
        );
      });
})