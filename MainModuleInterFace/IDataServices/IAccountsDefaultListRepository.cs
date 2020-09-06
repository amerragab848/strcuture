using System;
using System.Collections.Generic;
using System.Text;

namespace MainModuleInterFace.IDataServices
{
    //accounts_default_list data base entity 
    public interface IAccountsDefaultListRepository : IGenericRepository<accounts_default_list>
    {
        IEnumerable<accounts_default_list> selectByAccountIdType(int accountOwnerId, string listType);
        IEnumerable<accounts_default_list> selectByAccountIdTypeAbbreviation(string listType, int accountOwnerId, string abbreviation);
        IEnumerable<accounts_default_list> selectByAccountIdTypeAction(string list_type, int accountOwnerId, int action);
        IEnumerable<accounts_default_list> selectByAccountIdTypeNoaction(string list_type, int accountOwnerId);
        accounts_default_list selectById(int id);
        List<DtoListTypes> selectTypesOnly(string lang, int accountOwnerId);
        accounts_default_list selectByAccountIdTypeActionNotList(string list_type, int accountOwnerId, int action);
        IEnumerable<DtoAccountsDefaultList> selectTypes(int pageNumber, int pageSize, string lang);
        List<DtoAccountsDefaultList> selectByListType(string listType, int pageNumber, int pageSize, int accountOwnerId, string language);
        List<DtoSelectItems> selectByListTypeForList(string listType, int accountOwnerId, string language);
        List<DtoSelectItems> selectContainsByListType(List<string> listType, int accountOwnerId, string language);
        List<DtoAccountsDefaultList> selectByListTypeForselect(string listType, int accountOwnerId, string language);

        DtoAccountsDefaultList selectForEdit(int id);
        List<DtoAccountsDefaultList> GetExpensesWorkFlow(string lang);

        IEnumerable<DtoAccountsDefaultList> selectTypesNotEqualAction(string listType, int action, string lang);
        IEnumerable<DtoAccountsDefaultList> selectByListTypeWithAction(string listType, string lang);
        List<DtoAccountsDefaultList> selectDefaultList(string listType, string lang);
        List<DtoAccountsDefaultList> selectListType(string language);
    }
}
