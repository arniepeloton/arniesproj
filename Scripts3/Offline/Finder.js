define([], function() {

    window.eMajorDataType = {
        String: 1,
        Date: 2,
        Number: 3
    }; // MajorDataType

    window.Finder = function() {

        var mMyClass = this;
        this.Fields = new FinderFields();
        this.MaxRecords = 50;
        this.Distinct = false;
        this.SearchString = '';
        var mSqlSelect;
        var pField; // FinderField
        var pFindField; // string
        var pStr;
        this.SqlWhereExact = '';
        this.SqlWhere = '';
        this.SqlFrom = '';
        this.SqlGroup = '';
        this.SqlHaving = '';
        this.SqlOrder = '';

        this.CreateSql = function() {

            var pWhere = '';
            var pStr;

            if (CStr2(mMyClass.SqlWhereExact).length > 0) {
                pWhere = mMyClass.SqlWhereExact;
            } else {
                pWhere = mMyClass.CreateWhere();
                if (CStr2(mMyClass.SqlWhere).trim().length > 0) {
                    if (pWhere.length > 0) {
                        pWhere = mMyClass.SqlWhere + ' AND (' + pWhere + ')';
                    } else {
                        pWhere = mMyClass.SqlWhere;
                    };
                } else if (pWhere.length > 0) {
                    pWhere = ' WHERE ' + pWhere;
                };
            }; // if (CStr2(mMyClass.SqlWhereExact).length > 0) {

            CreateSqlSelect();
            pStr = mSqlSelect + ' ' +
                mMyClass.SqlFrom + ' ' +
                pWhere + ' ' +
                mMyClass.SqlGroup + ' ' +
                mMyClass.SqlHaving;

            pStr += ' ' + mMyClass.SqlOrder;
            if (CDec2(mMyClass.MaxRecords) > 0)
                pStr += ' LIMIT ' + mMyClass.MaxRecords;

            return pStr;

        }; // CreateSql

        function CreateSqlSelect() {

            var pSelect = "";
            var pStr;
            var pField; // FinderField

            for(pField in mMyClass.Fields.Items) {
                pField = mMyClass.Fields.Items[pField];
                if (CBool(pField.ExcludeFromSelect))
                    continue;
                if (pSelect != '') pSelect += ', ';
                pStr = '';
                if (pField.NoAlias) {
                    if ((CStr2(pField.Expression)).length > 0)
                        pStr = pField.Expression;
                    else
                        pStr += pField.FieldName;
                } else {
                    if (CStr2(pField.Expression).length > 0)
                        pStr = pField.Expression + ' AS ';
                        
                    pStr += '[' + pField.FieldName + ']';
                };
                pSelect += pStr + '\n';
            }; // for each 

            mSqlSelect = 'SELECT ';
            if (mMyClass.Distinct)
                mSqlSelect += 'DISTINCT ';
            mSqlSelect += pSelect;

        }; // CreateSqlSelect

        this.CreateWhere = function() {

            var pField; // FinderField
            var pFindField = ''; //  String
            var pStr; // String
            var pStrs = []; // String
            var pWhere = ''; // String
            var p = 0; // Integer

            if (CStr2(mMyClass.SearchString).length > 0) {
                for(pField in mMyClass.Fields.Items) {
                    pField = mMyClass.Fields.Items[pField];
                    if (pField.Searchable 
                        && pField.NoAlias == false) {

                        if (CStr2(pField.Expression).length > 0) {
                            pStr = pField.Expression;
                        } else {
                            pStr = pField.FieldName;
                        };

                        if (pFindField.length > 0) pFindField += ' || ';
                        if (pField.MajorDataType == eMajorDataType.String) {
                            pFindField += "IFNULL(" + pStr + ", '')";
                        } else {
                            pFindField += "IFNULL(" + pStr + ", '')"; // same as string. preserve ELSE branch
                        };
                    }; // if (pField.Searchable && pField.NoAlias == false)
                }; // for(pField in mMyClass.Fields.Items) {

                pStrs = mMyClass.SearchString.split(' ');

                for (p = 0; p <= pStrs.length - 1; p++) {
                    if (pWhere.length > 0) pWhere += ' AND ';
                    pWhere += pFindField + ' LIKE ' + ReadyString('%' + pStrs[p] + '%');
                }; // for (p = 0; p <= pStrs.length - 1; p++) {

                if (pWhere.length > 0) {
                    pWhere = ' (' + pWhere + ')';
                } else {
                    return '';
                }; // if (pWhere.length > 0) {
            }; //  if (CStr2(mMyClass.SearchString).length > 0) {

            return pWhere;

        }; // CreateWhere

        this.Requery = function() {

            var pSql;
            var pDT;

            pSql = mMyClass.CreateWhere(); // debug only
            pSql = mMyClass.CreateSql();
            pDT = Sql.Execute(pSql);

            return pDT;
            
        }; // Requery

        return this;

    }; // Finder

    window.FinderFields = function() {

        this.Items = [];

        this.Add = function(vFieldName, vMajorDataType) {

            if (CStr2(vFieldName).trim() == '')
                throw error('vFieldName must not be blank');

            var pOut = new FinderField();

            pOut.FieldName = vFieldName;
            if (vMajorDataType != undefined)
                pOut.MajorDataType = vMajorDataType;
            this.Items.push(pOut);

            return pOut;

        };

        return this;

    }; // FinderFields

    window.FinderField = function() {

        this.FieldName = '';
        this.MajorDataType = eMajorDataType.String;
        this.Searchable = true;
        this.ExcludeFromSelect = false;
        this.NoAlias = false;

        return this;

    }; // FinderField

}); // define