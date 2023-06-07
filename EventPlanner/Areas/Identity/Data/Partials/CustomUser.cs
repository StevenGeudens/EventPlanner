namespace EventPlanner.Areas.Identity.Data
{
    public partial class CustomUser
    {
        public override string ToString()
        {
            return $"{this.FirstName} {this.Name} - {this.Email}";
        }
    }
}
