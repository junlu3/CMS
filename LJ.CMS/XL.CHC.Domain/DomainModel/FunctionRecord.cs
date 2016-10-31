namespace XL.CHC.Domain.DomainModel
{
   public class FunctionRecord
   {
       public Guid Id{get;set;} 
       public Guid PermissionRecord_Id{get;set;} 
       public string Controller{get;set;} 
       public string Action{get;set;} 
       public bool?Deleted{get;set;} 
   }
}
