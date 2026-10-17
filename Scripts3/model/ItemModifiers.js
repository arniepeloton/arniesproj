define(['common'
    ], function() {
    CreateStringObject('App.model.ItemModifiers', null, function() {
        
        var mOut = {};

        mOut.ID = null;

        mOut.Add = function(vParams) {
            /*vParams = {
                Name: 'string',
                success: function(vData) {
                    vData = {
                        ID: 'id of the newly created LibraryItemModifiers
                    }
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/Add',
                data: {
                    Name: vParams.Name
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else if (IsGuid(vData.Result)) {
                        mOut.ID = vData.Result;
                        vParams.success.apply(this, arguments);
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });

        }; // END mOut.Add = function() {

        mOut.Rename = function(vParams) {
            /*vParams = {
                NewName: 'string',
                success: function(vData) {
                    vData.Result == 'ok';
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/Rename',
                data: {
                    ID: mOut.ID,
                    NewName: vParams.NewName
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.success.apply(this, arguments);
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });
        };

        mOut.AddCategory = function(vParams) {
            /*vParams = {
                CategoryName: 'string',
                CategoryNames: ['string1', 'string2'],
                success: function(vData) {
                    vData = {
                        
                    }
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/AddCategory',
                data: {
                    ID: mOut.ID,
                    CategoryName: vParams.CategoryName,
                    CategoryNames: vParams.CategoryNames
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.success();
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });
        }; // END mOut.AddCategory = function(vCategoryName) {

        mOut.DeleteCategories = function(vParams) {
            /*vParams = {
                CategoryNames: ['string1', 'string2'],
                success: function(vData) {
                    vData = {
                        
                    }
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/DeleteCategories',
                data: {
                    ID: mOut.ID,
                    CategoryNames: vParams.CategoryNames
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.success();
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });
        }; // END mOut.AddCategory = function(vCategoryName) {

        mOut.GetData = function(vParams) {
            /*vParams = {
                success: function(vData) {
                    vData = {
                        
                    }
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/GetData',
                data: {
                    ID: mOut.ID
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else
                        vParams.success(vData);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });
        }; // END mOut.AddCategory = function(vCategoryName) {

        mOut.AddRebate = function(vParams) {
            /*vParams = {
                RebateID: 'guid string' optional. set if updating a record. blank if inserting,
                ProductID: 'guid string',
                Amount: 123.456,
                UnitMeasure: 'string',
                UnitQuantity: numeric,
                Quantity: numeric,
                success: function(vData) {
                    vData = {
                        ID: 'id of the newly created LibraryItemModifiers
                    }
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/AddRebate',
                data: {
                    ItemModifierID: mOut.ID,
                    RebateID: vParams.RebateID,
                    ProductID: vParams.ProductID,
                    Amount: vParams.Amount,
                    UnitMeasure: vParams.UnitMeasure,
                    UnitQuantity: vParams.UnitQuantity,
                    Quantity: vParams.Quantity,
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else if (vData.Result) {
                        vParams.success(vData);
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });
        }; // END mOut.AddCategory = function(vCategoryName) {

        mOut.DeleteRebates = function(vParams) {
            /*vParams = {
                ProductIDs: ['string1', 'string2'],
                success: function(vData) {
                    vData = {
                        
                    }
                }, 
                error: function() {}
            }*/

            AjaxSend({
                url: '/ItemModifiers/DeleteRebates',
                data: {
                    ID: mOut.ID,
                    ProductIDs: vParams.ProductIDs
                },
                success: function(vData) {
                    if (vData.Error) 
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.success();
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function() {
                    vParams.error.apply(this, arguments);
                }
            });
        }; // END mOut.AddCategory = function(vCategoryName) {

        mOut.SaveItemModifier = function (vParams) {
            AjaxSend({
                url: '/ItemModifiers/SaveItemModifier',
                data: vParams.Data,
                success: function (vData) {
                    if (vData.Error)
                        vParams.error(vData.Error);
                    else if (vData.Result == 'ok') {
                        vParams.success();
                    } else
                        vParams.error.apply(this, arguments);
                },
                error: function () {
                    vParams.error.apply(this, arguments);
                }
            });
        }

        return mOut;
    });

    App.model.ItemModifiers.Search = function(vParams) {
        /*vParams = {
            Keywords: 'string',
            success: function(vData) {},
            error: function(vData) {}
        }*/

        AjaxSend({
            url: '/Browser/ItemModifiers',
            data: {
                keywords: vParams.Keywords
            },
            success: function(vData) {
                vParams.success.apply(this, arguments);
            },
            error: vParams.error
        });

    }; // END App.model.ItemModifiers.Search = function(vParams) {

    App.model.ItemModifiers.CategorySearch = function(vParams) {
        /*vParams = {
            Keywords: 'string',
            success: function(vData) {},
            error: function(vData) {}
        }*/

        AjaxSend({
            url: '/Browser/PimCategories',
            data: {
                keywords: vParams.Keywords
            },
            success: function(vData) {
                vParams.success.apply(this, arguments);
            },
            error: vParams.error
        });
    };

    App.model.ItemModifiers.GetPimData = function(vParams) {
        /*vParams = {
            ProductID: 'string-guid',
            success: function(vData) {
                vData = {
                        
                }
            }, 
            error: function() {}
        }*/

        AjaxSend({
            url: '/ItemModifiers/GetPimData',
            data: {
                ProductID: vParams.ProductID
            },
            success: function(vData) {
                if (vData.Error) 
                    vParams.error(vData.Error);
                else if (vData.data) {
                    vParams.success(vData);
                } else
                    vParams.error.apply(this, arguments);
            },
            error: function() {
                vParams.error.apply(this, arguments);
            }
        });
    }; // END mOut.GetPimData

    App.model.ItemModifiers.CategorySearchTest = function() {
        App.model.ItemModifiers.CategorySearch({
            Keywords: 'o',
            success: function(vData) {
                console.table(vData.Results.Rows);
            }
        });
    };

    App.model.ItemModifiers.AddCategoryTest = function() {
        var p = new App.model.ItemModifiers();
        p.ID = '{D7BE5A3E-BB58-4F40-8398-470BE5E79314}';
        p.AddCategory({
            CategoryName: 'Beverages and Desserts',
            CategoryNames: ['GLAMOUR SERVICES > EYELASH PERMING',
                'Facial and Cleaning > Add on Masques',
                'Seafoods & Poultry'
            ],
            success: function(vData) {
                console.log('jsLJ5CDQ4FCD! success');
            },
            error: function(vData) {
                console.log('jsCMHER4ZR75! ' + JSON.stringify(vData));
            }
        });
    };

    App.model.ItemModifiers.GetDataTest = function() {
        var p = new App.model.ItemModifiers();
        p.ID = '{D7BE5A3E-BB58-4F40-8398-470BE5E79314}';
        p.GetData({
            success: function(vData) {
                console.table(vData.Categories.Rows);
                console.table(vData.Rebates.Rows);
            },
            error: function(vData) {
                console.log('jsLSMLZ68WAK! ' + JSON.stringify(vData));
            }
        });
    };

    App.model.ItemModifiers.AddRebateTest = function() {
        var p = new App.model.ItemModifiers();
        p.ID = '{D7BE5A3E-BB58-4F40-8398-470BE5E79314}';
        p.AddRebate({
            ProductID: '815a1bb8-b793-4582-a4bf-f71c1adf97ff',
            Amount: 29.75,
            UnitMeasure: 'dangkal',
            UnitQuantity: 13,
            Quantity: 2,
            success: function(vData) {
                console.log('jsNH539XP5BC! ' + IsGuid(vData.Result));
            },
            error: function(vData) {
                console.log('jsCPLSNTM9M6! ' + JSON.stringify(vData));
            }
        });

        p.AddRebate({
            ProductID: 'f0381f02-9267-4b50-9845-191fb950c46d',
            Amount: 29.75,
            success: function(vData) {
                console.log('jsNH539XP5BC! ' + IsGuid(vData.Result));
            },
            error: function(vData) {
                console.log('jsCPLSNTM9M6! ' + JSON.stringify(vData));
            }
        });

        p.AddRebate({
            ProductID: '6dbcccd4-ed4e-4b48-bbf4-b1879b0a462b',
            Amount: 29.75,
            success: function(vData) {
                console.log('jsNH539XP5BC! ' + IsGuid(vData.Result));
            },
            error: function(vData) {
                console.log('jsCPLSNTM9M6! ' + JSON.stringify(vData));
            }
        });
    };

    App.model.ItemModifiers.DeleteCategoriesTest = function() {
        var p = new App.model.ItemModifiers();
        p.ID = '{D7BE5A3E-BB58-4F40-8398-470BE5E79314}';
        p.DeleteCategories({
            CategoryNames: ['GLAMOUR SERVICES > EYELASH PERMING',
                'Seafoods & Poultry'
            ],
            success: function(vData) {
                console.log('js75TJK9NASM! success');
            },
            error: function(vData) {
                console.log('jsZSRAGX3PZ6! ' + JSON.stringify(vData));
            }
        });
    };

    App.model.ItemModifiers.DeleteRebatesTest = function() {
        var p = new App.model.ItemModifiers();
        p.ID = '{D7BE5A3E-BB58-4F40-8398-470BE5E79314}';
        p.DeleteRebates({
            ProductIDs: ['815a1bb8-b793-4582-a4bf-f71c1adf97ff',
                '6dbcccd4-ed4e-4b48-bbf4-b1879b0a462b'
            ],
            success: function(vData) {
                console.log('jsAPYRBZR9CX! success');
            },
            error: function(vData) {
                console.log('jsK5K86TCWYF! ' + JSON.stringify(vData));
            }
        });
    };

    App.model.ItemModifiers.GetPimDataTest = function() {
        App.model.ItemModifiers.GetPimData({
            ProductID: '815a1bb8-b793-4582-a4bf-f71c1adf97ff',
            success: function(vData) {
                console.table(vData.data.Rows);
            },
            error: function(vData) {
                console.log('jsK5K86TCWYF! ' + JSON.stringify(vData));
            }
        });
    };

    App.model.ItemModifiers.RenameTest = function() {
        var p = new App.model.ItemModifiers();
        p.ID = '{AAC388C8-5341-4645-B312-F2923D47E951}';
        p.Rename({
            NewName: 'New Name Test ' + RandomInteger(1, 999999),
            success: function(vData) {
                console.log('js22ND8PZM4V! success');
            },
            error: function(vData) {
                console.log('js55F7N5ST46! ' + JSON.stringify(vData));
            }
        });
    };

});