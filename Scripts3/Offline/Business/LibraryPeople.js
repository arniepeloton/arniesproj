define([], function() {
    window.Business.LibraryPeople = new function(vParent) {
        this.ByID = function(vID) {

            var pDR;
            var pSql;

            pSql = 'SELECT ppol.ID\n' +
                ',ppol.DTRPIN\n' +
                ",IFNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) || ' ', ' '), '') || " +
                "IFNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) || ', ',', '), '') || " +
                "IFNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) || ' ',' '), '') || " +
                "IFNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) || ' ',' '), '') || " +
                "IFNULL(NULLIF(', ' || LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') AS FullName"; 

            pSql += ' FROM LibraryPeople ppol\n' +
                ' LEFT JOIN LibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)';

            pSql += ' WHERE ppol.ID = ' + ReadyString(vID);
             
             pDR = Sql.ExecuteRow(pSql);

             return pDR;
        };

        return this;
    };
});