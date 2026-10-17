@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/Plain.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/jquery.mobile.flatui.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    
    <script>                
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/DashBoardsAndReports.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/crm.js?crc=')]);
        setTimeout(function () {            
            //require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
        }, 3000);        
    </script>
    

End Section

@*Header*@
<div data-role="page">
    <div data-role="header" data-theme="a">
        <div class="ui-grid-a">
            <div class="ui-block-a" style="text-align:center; padding-right:9px;">
                <span>
                    <br />
                    Company Name
                </span>
            </div>
            <div class="ui-block-b" style="text-align:center; padding-right:9px;">
                <input type="text" placeholder="Search here"/>
            </div>
        </div>
    </div>
    <br />
    <div class="ui-grid-a ui-responsive">
        <div class="ui-block-a thirtyseventy-block-a">
            <a id="btnPeriodPopup"  data-rel="popup" data-icon="arrow-r" data-iconpos="right" data-corners="false" data-theme="c" data-role="button" data-position-to="window">
                Period <span id="lblPeriod" style="font-weight:bold">[ Today ]</span>
            </a>
            <div data-role="popup" id="periodpopup-screen" class="periodpopup" data-overlay-theme="a" data-corners="false" data-tolerance="30,15">
                <a href="#" data-rel="back" class="ui-btn ui-corner-all ui-shadow ui-btn-a ui-icon-delete ui-btn-icon-notext ui-btn-right">Close</a>
                <center><h3>Period</h3></center>
                <div data-role="content">
                    <ul data-role="listview" data-theme="f" class="ui-listview ui-listview-inset ui-shadow">
                        <li id="li2DaysAgo" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="f" class="ui-btn ui-btn-icon-right ui-icon-carat-r">
                            2 Days Ago
                        </li>
                        <li id="liYesterday" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="f" class="ui-btn ui-btn-icon-right ui-icon-carat-r">
                            Yesterday
                        </li>
                        <li id="liToday" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="f" class="ui-btn ui-btn-icon-right ui-icon-carat-r">
                            Today
                        </li>
                        <li id="liWeekToDate" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="f" class="ui-btn ui-btn-icon-right ui-icon-carat-r">
                            Week To Date
                        </li>
                        <li id="liMonthToDate" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="f" class="ui-btn ui-btn-icon-right ui-icon-carat-r">
                            Month To Date
                        </li>
                        <li id="liCustomPeriod" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="f" class="ui-btn ui-btn-icon-right ui-icon-carat-r">
                            Custom Period
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="ui-block-b thirtyseventy-block-b">
            <div class="ui-grid-a" id="forcustomperiod" style="display:none; margin-top:7px;">
                <div class="ui-block-a">
                    <div class="ui-grid-a ui-responsive">
                        <div class="ui-block-a customperiod-block-a">
                            <span style="margin-left:5px;">From:</span>
                        </div>
                        <div class="ui-block-b customperiod-block-b customperiodcontrolgroup">
                            <div data-role="controlgroup" data-corners="false" data-type="horizontal" style="margin-top:-5px;">
                                <select id="Mon" data-theme="c">
                                    <option>MMM</option>
                                    <option value="0">Jan</option>
                                    <option value="1">Feb</option>
                                    <option value="2">Mar</option>
                                    <option value="3">Apr</option>
                                    <option value="4">May</option>
                                    <option value="5">Jun</option>
                                    <option value="6">Jul</option>
                                    <option value="7">Aug</option>
                                    <option value="8">Sep</option>
                                    <option value="9">Oct</option>
                                    <option value="10">Nov</option>
                                    <option value="11">Dec</option>
                                </select>
                                <select id="Day" data-theme="c"><option>DD</option><option>1</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option><option>12</option><option>13</option><option>14</option><option>15</option><option>16</option><option>17</option><option>18</option><option>19</option><option>20</option><option>21</option><option>22</option><option>23</option><option>24</option><option>25</option><option>26</option><option>27</option><option>28</option><option>29</option><option>30</option><option>31</option></select>
                                <select id="Year" data-theme="c"><option>YYYY</option><option>1900</option><option>1901</option><option>1902</option><option>1903</option><option>1904</option><option>1905</option><option>1906</option><option>1907</option><option>1908</option><option>1909</option><option>1910</option><option>1911</option><option>1912</option><option>1913</option><option>1914</option><option>1915</option><option>1916</option><option>1917</option><option>1918</option><option>1919</option><option>1920</option><option>1921</option><option>1922</option><option>1923</option><option>1924</option><option>1925</option><option>1926</option><option>1927</option><option>1928</option><option>1929</option><option>1930</option><option>1931</option><option>1932</option><option>1933</option><option>1934</option><option>1935</option><option>1936</option><option>1937</option><option>1938</option><option>1939</option><option>1940</option><option>1941</option><option>1942</option><option>1943</option><option>1944</option><option>1945</option><option>1946</option><option>1947</option><option>1948</option><option>1949</option><option>1950</option><option>1951</option><option>1952</option><option>1953</option><option>1954</option><option>1955</option><option>1956</option><option>1957</option><option>1958</option><option>1959</option><option>1960</option><option>1961</option><option>1962</option><option>1963</option><option>1964</option><option>1965</option><option>1966</option><option>1967</option><option>1968</option><option>1969</option><option>1970</option><option>1971</option><option>1972</option><option>1973</option><option>1974</option><option>1975</option><option>1976</option><option>1977</option><option>1978</option><option>1979</option><option>1980</option><option>1981</option><option>1982</option><option>1983</option><option>1984</option><option>1985</option><option>1986</option><option>1987</option><option>1988</option><option>1989</option><option>1990</option><option>1991</option><option>1992</option><option>1993</option><option>1994</option><option>1995</option><option>1996</option><option>1997</option><option>1998</option><option>1999</option><option>2000</option><option>2001</option><option>2002</option><option>2003</option><option>2004</option><option>2005</option><option>2006</option><option>2007</option><option>2008</option><option>2009</option><option>2010</option><option>2011</option><option>2012</option><option>2013</option><option>2014</option></select>
                            </div>
                            <div style="clear:both;"></div>
                        </div>
                    </div>
                </div>
                <div class="ui-block-b">
                    <div class="ui-grid-a ui-responsive">
                        <div class="ui-block-a customperiod-block-a">
                            <span style="margin-left:5px;">To:</span>
                        </div>
                        <div class="ui-block-b customperiod-block-b customperiodcontrolgroup">
                            <div data-role="controlgroup" data-corners="false" data-type="horizontal" style="margin-top:-5px;">
                                <select id="Mon" data-theme="c">
                                    <option>MMM</option>
                                    <option value="0">Jan</option>
                                    <option value="1">Feb</option>
                                    <option value="2">Mar</option>
                                    <option value="3">Apr</option>
                                    <option value="4">May</option>
                                    <option value="5">Jun</option>
                                    <option value="6">Jul</option>
                                    <option value="7">Aug</option>
                                    <option value="8">Sep</option>
                                    <option value="9">Oct</option>
                                    <option value="10">Nov</option>
                                    <option value="11">Dec</option>
                                </select>
                                <select id="Day" data-theme="c"><option>DD</option><option>1</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option><option>12</option><option>13</option><option>14</option><option>15</option><option>16</option><option>17</option><option>18</option><option>19</option><option>20</option><option>21</option><option>22</option><option>23</option><option>24</option><option>25</option><option>26</option><option>27</option><option>28</option><option>29</option><option>30</option><option>31</option></select>
                                <select id="Year" data-theme="c"><option>YYYY</option><option>1900</option><option>1901</option><option>1902</option><option>1903</option><option>1904</option><option>1905</option><option>1906</option><option>1907</option><option>1908</option><option>1909</option><option>1910</option><option>1911</option><option>1912</option><option>1913</option><option>1914</option><option>1915</option><option>1916</option><option>1917</option><option>1918</option><option>1919</option><option>1920</option><option>1921</option><option>1922</option><option>1923</option><option>1924</option><option>1925</option><option>1926</option><option>1927</option><option>1928</option><option>1929</option><option>1930</option><option>1931</option><option>1932</option><option>1933</option><option>1934</option><option>1935</option><option>1936</option><option>1937</option><option>1938</option><option>1939</option><option>1940</option><option>1941</option><option>1942</option><option>1943</option><option>1944</option><option>1945</option><option>1946</option><option>1947</option><option>1948</option><option>1949</option><option>1950</option><option>1951</option><option>1952</option><option>1953</option><option>1954</option><option>1955</option><option>1956</option><option>1957</option><option>1958</option><option>1959</option><option>1960</option><option>1961</option><option>1962</option><option>1963</option><option>1964</option><option>1965</option><option>1966</option><option>1967</option><option>1968</option><option>1969</option><option>1970</option><option>1971</option><option>1972</option><option>1973</option><option>1974</option><option>1975</option><option>1976</option><option>1977</option><option>1978</option><option>1979</option><option>1980</option><option>1981</option><option>1982</option><option>1983</option><option>1984</option><option>1985</option><option>1986</option><option>1987</option><option>1988</option><option>1989</option><option>1990</option><option>1991</option><option>1992</option><option>1993</option><option>1994</option><option>1995</option><option>1996</option><option>1997</option><option>1998</option><option>1999</option><option>2000</option><option>2001</option><option>2002</option><option>2003</option><option>2004</option><option>2005</option><option>2006</option><option>2007</option><option>2008</option><option>2009</option><option>2010</option><option>2011</option><option>2012</option><option>2013</option><option>2014</option></select>
                            </div>
                            <div style="clear:both;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="ui-grid-a ui-responsive">
        <div class="ui-block-a thirtyseventy-block-a" style="padding:5px;">
            <div class="ui-grid-solo">
                <div class="ui-block-a">
                    <a data-role="button" data-theme="b" data-corners="false" style="margin-left:-4px; height:auto;">
                        <div style="padding:5px;font-weight:normal; font-size:30px; margin:0px auto; text-align:left">
                            <div style="font-weight:bold; font-size:32px; text-align:center;">Name Here</div>
                            <hr style="border:1px solid #297E6D" />

                            <div style="text-align:left; font-size:16px">Net Sales</div>
                            <div style="text-align:right; font-size:32px;">P 10,000</div>

                            <div style="text-align:left; font-size:16px">Gross Sales</div>
                            <div style="text-align:right; font-size:25px;">10,000</div>

                            <div style="text-align:left; font-size:16px">Discounts</div>
                            <div style="text-align:right; font-size:25px;">0</div>

                            <div style="text-align:left; font-size:16px">Refunds</div>
                            <div style="text-align:right; font-size:25px;">5,000</div>

                            <div style="text-align:left; font-size:16px">Exchanges</div>
                            <div style="text-align:right; font-size:25px;">3,000</div>

                            <div style="text-align:left; font-size:16px">FOWST COUNT:</div>
                            <div style="text-align:right; font-size:25px;">6</div>
                        </div>
                        <div class="nothingdiv" style="height:102px;"></div>
                    </a>
                </div>
            </div>
        </div>
        <div class="ui-block-b thirtyseventy-block-b" style="padding:5px;">
            <div style="/*overflow:auto; height:224px;*/">
                <div class="ui-grid-b">
                    <div class="ui-block-a">
                        <a id="btnView" data-corners="false" data-role="button" data-theme="c">
                            <div style="padding:5px;font-weight:normal; font-size:20px; margin:0px auto;">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">GMA</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:left; font-size:13px">Net Sales</div>
                                <div style="text-align:right; font-size:15px;">P 10,000</div>

                                <div style="text-align:left; font-size:13px">Gross Sales</div>
                                <div style="text-align:right; font-size:15px;">10,000</div>

                                <div style="text-align:left; font-size:13px">Discounts</div>
                                <div style="text-align:right; font-size:15px;">0</div>

                                <div style="text-align:left; font-size:13px">Refunds</div>
                                <div style="text-align:right; font-size:15px;">5,000</div>

                                <div style="text-align:left; font-size:13px">Exchanges</div>
                                <div style="text-align:right; font-size:15px;">3,000</div>
                            </div>
                        </a>
                    </div>
                    <div class="ui-block-b">
                        <a data-corners="false" data-role="button" data-theme="c">
                            <div style="padding:5px;font-weight:normal; font-size:20px; margin:0px auto;">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">NCR</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:left; font-size:13px">Net Sales</div>
                                <div style="text-align:right; font-size:15px;">P 10,000</div>

                                <div style="text-align:left; font-size:13px">Gross Sales</div>
                                <div style="text-align:right; font-size:15px;">10,000</div>

                                <div style="text-align:left; font-size:13px">Discounts</div>
                                <div style="text-align:right; font-size:15px;">0</div>

                                <div style="text-align:left; font-size:13px">Refunds</div>
                                <div style="text-align:right; font-size:15px;">5,000</div>

                                <div style="text-align:left; font-size:13px">Exchanges</div>
                                <div style="text-align:right; font-size:15px;">3,000</div>
                            </div>
                        </a>
                    </div>
                    <div class="ui-block-c">
                        <a data-corners="false" data-role="button" data-theme="c">
                            <div style="padding:5px;font-weight:normal; font-size:20px; margin:0px auto;">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">VISAYAS</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:left; font-size:13px">Net Sales</div>
                                <div style="text-align:right; font-size:15px;">P 10,000</div>

                                <div style="text-align:left; font-size:13px">Gross Sales</div>
                                <div style="text-align:right; font-size:15px;">10,000</div>

                                <div style="text-align:left; font-size:13px">Discounts</div>
                                <div style="text-align:right; font-size:15px;">0</div>

                                <div style="text-align:left; font-size:13px">Refunds</div>
                                <div style="text-align:right; font-size:15px;">5,000</div>

                                <div style="text-align:left; font-size:13px">Exchanges</div>
                                <div style="text-align:right; font-size:15px;">3,000</div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="ui-grid-b">
                    <div class="ui-block-a">
                        <a data-corners="false" data-role="button" data-theme="c">
                            <div style="padding:5px;font-weight:normal; font-size:20px; margin:0px auto;">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">MINDANAO</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:left; font-size:13px">Net Sales</div>
                                <div style="text-align:right; font-size:15px;">P 10,000</div>

                                <div style="text-align:left; font-size:13px">Gross Sales</div>
                                <div style="text-align:right; font-size:15px;">10,000</div>

                                <div style="text-align:left; font-size:13px">Discounts</div>
                                <div style="text-align:right; font-size:15px;">0</div>

                                <div style="text-align:left; font-size:13px">Refunds</div>
                                <div style="text-align:right; font-size:15px;">5,000</div>

                                <div style="text-align:left; font-size:13px">Exchanges</div>
                                <div style="text-align:right; font-size:15px;">3,000</div>
                            </div>
                        </a>
                    </div>
                    <div class="ui-block-b">
                        <a data-corners="false" data-role="button" data-theme="c">
                            <div style="padding:5px;font-weight:normal; font-size:20px; margin:0px auto;">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">OVERSEAS</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:left; font-size:13px">Net Sales</div>
                                <div style="text-align:right; font-size:15px;">P 10,000</div>

                                <div style="text-align:left; font-size:13px">Gross Sales</div>
                                <div style="text-align:right; font-size:15px;">10,000</div>

                                <div style="text-align:left; font-size:13px">Discounts</div>
                                <div style="text-align:right; font-size:15px;">0</div>

                                <div style="text-align:left; font-size:13px">Refunds</div>
                                <div style="text-align:right; font-size:15px;">5,000</div>

                                <div style="text-align:left; font-size:13px">Exchanges</div>
                                <div style="text-align:right; font-size:15px;">3,000</div>
                            </div>
                        </a>
                    </div>
                    <div class="ui-block-c">
                        <a data-corners="false" data-role="button" data-theme="c">
                            <div style="padding:5px;font-weight:normal; font-size:20px; margin:0px auto;">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">UNSORTED</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:left; font-size:13px">Net Sales</div>
                                <div style="text-align:right; font-size:15px;">P 10,000</div>

                                <div style="text-align:left; font-size:13px">Gross Sales</div>
                                <div style="text-align:right; font-size:15px;">10,000</div>

                                <div style="text-align:left; font-size:13px">Discounts</div>
                                <div style="text-align:right; font-size:15px;">0</div>

                                <div style="text-align:left; font-size:13px">Refunds</div>
                                <div style="text-align:right; font-size:15px;">5,000</div>

                                <div style="text-align:left; font-size:13px">Exchanges</div>
                                <div style="text-align:right; font-size:15px;">3,000</div>
                            </div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@*</div>*@