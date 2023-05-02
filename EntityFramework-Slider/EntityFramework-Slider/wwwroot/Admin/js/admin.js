$(function () {

    $(document).on("click",".slider-status", function () {


        let sliderId = $(this).parent().attr("data-id");
        let changeElem = $(this);
        let data = { id: sliderId }

        $.ajax({      //ajax for deactive-active Action

            url: "slider/setstatus",
            type: "Post",
            data: data,
            success: function (res) {

                if (res) {
                    $(changeElem).removeClass("active-status");
                    $(changeElem).addClass("deActive-status");
                } else {
                    $(changeElem).removeClass("deActive-status");
                    $(changeElem).addClass("active-status");
                }
          
            }
        })
    })

   //-------SOFTDELETE DATA IN ADMIN PANEL WITHOUT REFRESH---------------

    $(document).on("submit", "#category-delete-form", function (e) {    //add-basket butona basdiqda islesin

        e.preventDefault();
        let categoryId = $(this).attr("data-id")   //hemin add-basket butonuna aid olan productun Id-sini goturuk
        let deletElem = $(this);
        let data = { id: categoryId }  //objexte beraber edirik bucur gonderik actiona productun Id-sini(bezpraktis).
        $.ajax({

            url: "category/softdelete",     //add-basket Action request atiriq
            type: "Post",
            data: data,
            success: function (res) {

                $(deletElem).parent().parent().remove();

            }
        })
  


    })





})