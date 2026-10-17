<script>
    require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/LoyaltyProgram.js?crc=')]);
</script>

<div data-role="content">

    <div>
        <ul data-role="listview" data-inset="true">

            <li data-role="list-divider">
                <div data-role="fieldcontain">
                    Points-to-Currency Equivalence (During Redemption of Points)
                </div>
            </li>
            <li>
                <div data-role="fieldcontain">
                    <label for="txtPointEquivalence" style="font-weight:bold">1 Point is equal to </label>
                    <input type="text" readonly="readonly" id="txtPointEquivalence" value="1.00" /> 
                </div>
                                
            </li>
        </ul>

        <ul data-role="listview" data-inset="true">

            <li data-role="list-divider">
                <div data-role="fieldcontain">
                    Points Awarded According to Payment Type
                </div>
            </li>
            <li>
                <table>
                    <tr>
                        <td>
                            Every Cash (net) sale worth
                        </td>
                        <td>
                            <table>
                               <tr>
                                   <td>
                                       P
                                   </td>
                                   <td>
                                       <input type="number" id="txtCash" />
                                   </td>
                               </tr>
                            </table>                            
                        </td>
                        <td style="text-align:center"> gets </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="number" id="txtCashPoint" /> 
                                    </td>
                                    <td>
                                        &nbsp; Point
                                    </td>
                                </tr>
                            </table>                                                        
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Every Cheque (net) sale worth
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        P
                                    </td>
                                    <td>
                                        <input type="number" id="txtCheque" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align:center"> gets </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="number" id="txtChequePoint" />
                                    </td>
                                    <td>
                                        &nbsp; Point
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Every Debit Card (net) sale worth
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        P
                                    </td>
                                    <td>
                                        <input type="number" id="txtDebitCard" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align:center"> gets </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="number" id="txtDebitCardPoint" />
                                    </td>
                                    <td>
                                        &nbsp; Point
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Every Credit Card (net) sale worth
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        P
                                    </td>
                                    <td>
                                        <input type="number" id="txtCreditCard" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align:center"> gets </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="number" id="txtCreditCardPoint" />
                                    </td>
                                    <td>
                                        &nbsp; Point
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Every SRV (net) sale worth
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        P
                                    </td>
                                    <td>
                                        <input type="number" id="txtSRV" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align:center"> gets </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="number" id="txtSRVPoint" />
                                    </td>
                                    <td>
                                        &nbsp; Point
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Every Gift Cerificate (net) sale worth
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        P
                                    </td>
                                    <td>
                                        <input type="number" id="txtGiftCerificate" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align:center"> gets </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="number" id="txtGiftCerificatePoint" />
                                    </td>
                                    <td>
                                        &nbsp; Point
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </li>
        </ul>

    </div>

    <br />
    <button id="btnLoyaltyProgramSettings"><h3>Save</h3></button>

    <a id="btnDone" data-role="button"><h3>Cancel</h3></a>
</div>