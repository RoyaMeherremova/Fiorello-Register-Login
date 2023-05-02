namespace EntityFramework_Slider.Heplers
{
    public class Paginate<T>  //paginate yaradiriq her model ucun paginate yazanda istifade edek deye Generik edirik
    {
        public List<T> Datas { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }

        public Paginate(List<T> datas,int currentpage,int totalPage)
        {
            Datas = datas;
            CurrentPage = currentpage;
            TotalPage = totalPage;
        }
        public bool HasPrevious  //eyer icinde olduqumu seyfe 1 den boyukduse previousu yani onceki seyfesin olsun.True qaytaracaq bize bu properti.
        {
            get
            {
                return CurrentPage > 1; 
            }
        }
        public bool HasNext  //eyer icinde olduqumu seyfe umumi seyfelerden azdisa True qaytaracaq bize bu properti.
        {
            get
            {
                return CurrentPage < TotalPage;
            }
        }


    }
}
