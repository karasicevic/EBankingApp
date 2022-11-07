//using ConsoleTableExt;
using EBankingApp.DataAccessLayer;
using EBankingApp.Model;
using PowerArgs;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

// Zbog cirilice
Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;
// Umesto VARCHAR smo u DDL-u za Ime i prezime stavili NVARCHAR da bi sql server mogao da cuva cirilicne karaktere

while (true)
{
    try
    {
        ShowMainMenu();
        var mainOption = Console.ReadLine();

        switch (mainOption)
        {
            case "0":
                {
                    Console.Clear();
                    Console.WriteLine("- Крај рада -");
                    return;
                }

            case "1":
                {
                    await UserUseCases();
                    break;
                }
            case "2":
                {
                    await AccountUseCases();
                    break;
                }
            case "3":
                {
                    await TransactionUseCases();
                    break;
                }
            case "4":
                {
                    await CurrencyUseCases();
                    break;
                }
            default:
                {
                    Console.WriteLine("Непозната опција. Покушајте поново..(притисните било који тастер за наставак)");
                    Console.ReadKey();
                    break;
                }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Грешка: {ex.Message} {Environment.NewLine} (притисните било који тастер за наставак)");
        Console.ReadKey();
    }
}

void ValidateCurrencyName(Currency newCurrency)
{
    if (string.IsNullOrWhiteSpace(newCurrency.Name))
        throw new Exception("Морате унети назив валуте");

    if (newCurrency.Name.Length > 100)
        throw new Exception("Назив валуте не сме имати више од 100 карактера");

    if (newCurrency.Name.Length < 2)
        throw new Exception("Назив валуте мора садржати бар два карактера");

}

void ValidateUserData(User newUser)
{
    ValidateFirstName(newUser);
    ValidateLastName(newUser);
    ValidateEmail(newUser);
    ValidatePassword(newUser);
}

void ValidateFirstName(User newUser)
{
    if (string.IsNullOrWhiteSpace(newUser.FirstName))
        throw new Exception("Морате унети име корисника");

    if (newUser.FirstName.Length > 100)
        throw new Exception("Име корисника не сме имати више од 100 карактера");

    if (newUser.FirstName.Length < 2)
        throw new Exception("Име корисника мора садржати бар два карактера");

    var regex = new Regex(@"^[АБВГДЂЕЖЗИЈКЛЉМНЊОПРСТЋУФХЦЧЏШ][абвгдђежзијклљмнњопрстћуфхцчџш]+([ -]{0,1}[АБВГДЂЕЖЗИЈКЛЉМНЊОПРСТЋУФХЦЧЏШ][абвгдђежзијклљмнњопрстћуфхцчџш]+)*$");

    if (regex.IsMatch(newUser.FirstName) == false)
        throw new Exception("Име корисника мора бити написано ћириличним писмом и прво слово мора бити велико");
}

void ValidateLastName(User newUser)
{
    if (string.IsNullOrWhiteSpace(newUser.LastName))
        throw new Exception("Морате унети презиме корисника");

    if (newUser.LastName.Length > 100)
        throw new Exception("Презиме корисника не сме имати више од 100 карактера");

    if (newUser.LastName.Length < 2)
        throw new Exception("Презиме корисника мора садржати бар два карактера");

    var regex = new Regex(@"^[АБВГДЂЕЖЗИЈКЛЉМНЊОПРСТЋУФХЦЧЏШ][абвгдђежзијклљмнњопрстћуфхцчџш]+([ -]{0,1}[АБВГДЂЕЖЗИЈКЛЉМНЊОПРСТЋУФХЦЧЏШ][абвгдђежзијклљмнњопрстћуфхцчџш]+)*$");

    if (regex.IsMatch(newUser.LastName) == false)
        throw new Exception("Презиме корисника мора бити написано ћириличним писмом и прво слово мора бити велико");
}

void ValidateEmail(User newUser)
{
    if (string.IsNullOrWhiteSpace(newUser.Email))
        throw new Exception("Морате унети мејл корисника");

    if (newUser.Password.Length > 100)
        throw new Exception("Мејл не сме имати више од 100 карактера");

    if (newUser.Password.Length < 5)
        throw new Exception("Мејл мора садржати бар 5 карактера");


    bool isEmail = Regex.IsMatch(newUser.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

    if (isEmail == false)
        throw new Exception("Мејл није у добром формату!");
}

void ValidatePassword(User newUser)
{
    if (string.IsNullOrWhiteSpace(newUser.LastName))
        throw new Exception("Морате унети лозинку корисника");

    if (newUser.Password.Length > 20)
        throw new Exception("Лозинка не сме имати више од 20 карактера");

    if (newUser.Password.Length < 7)
        throw new Exception("Лозинка корисника мора садржати бар осам карактера");

    var regex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).");

    if (regex.IsMatch(newUser.Password) == false)
        throw new Exception("Лозинка мора имати барем један специјални карактер," +
            " бар jедно велико слово, бар једно мало слово и бар један број");
}

void ValidateAccountNumber(Account newAccount)
{
    if (string.IsNullOrWhiteSpace(newAccount.Number))
        throw new Exception("Морате унети број рачуна");

    if (newAccount.Number.Length > 20)
        throw new Exception("Број рачуна не сме имати више од двадесет карактера");

    if (newAccount.Number.Length < 9)
        throw new Exception("Број рачуна мора садржати бар девет карактера");

}

void ShowMainMenu()
{
    Console.Clear();
    Console.WriteLine("1. Корисници");
    Console.WriteLine("2. Рачуни");
    Console.WriteLine("3. Трансакције");
    Console.WriteLine("4. Валуте");
    Console.WriteLine("0. Крај");
    Console.Write("Одаберите опцију: ");
}

async Task UserUseCases()
{
    var goBackRequested = false;
    while (goBackRequested == false)
    {
        try
        {
            ShowUserMenu();
            var userOption = Console.ReadLine();
            Console.Clear();
            switch (userOption)
            {
                case "0":
                    {
                        goBackRequested = true;
                        break;
                    }
                case "1":
                    {
                        Console.WriteLine("Унесите име:");
                        var firstName = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите презиме:");
                        var lastName = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите корисничку адресу:");
                        var email = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите шифру:");
                        var password = Console.ReadLine() ?? "";

                        var newUser = new User()
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            Password = password
                        };

                        ValidateUserData(newUser);

                        var user = await SqlRepository.CreateUser(newUser);

                        Console.WriteLine($"Додат нови корисник: '{user}'. (притисните било који тастер за наставак)");
                        Console.ReadKey();

                        break;
                    }
                case "2":
                    {
                        Console.Write("Унесите Ид корисника ког желите да измените: ");

                        if (!int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            throw new Exception("Погрешно сте унели Ид!");

                        }

                        Console.WriteLine("Унесите име:");
                        var firstName = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите презиме:");
                        var lastName = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите корисничку адресу:");
                        var email = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите шифру:");
                        var password = Console.ReadLine() ?? "";

                        var updUser = new User()
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            Password = password
                        };

                        ValidateUserData(updUser);

                        var user = await SqlRepository.UpdateUser(updUser,updateId);
                        if (user != null)
                        {
                            Console.WriteLine($"Корисник коме је ид {updateId} је успешно измењен. (притисните било који тастер за наставак)");
                        }
                        else
                        {
                            Console.WriteLine("Грешка приликом измене корисника");
                        }
                            Console.ReadKey();
                        break;
                    }
                case "3":
                    {
                        Console.Write("Унесите Ид корисника ког желите да орбишете: ");

                        if (!int.TryParse(Console.ReadLine(), out int delId))
                        {
                            throw new Exception("Погрешно сте унели Ид!");

                        }

                        await SqlRepository.DeleteUser(delId);
                        Console.WriteLine("Корисник је успешно обрисан. (притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
                case "4":
                    {
                        Console.Write("Унесите ИД корисника ког желите да прикажете: ");
                        if (!int.TryParse(Console.ReadLine(), out int idKorisnika))
                        {
                            throw new Exception("Погрешно сте унели Ид!");
                        }
                        var user = await SqlRepository.GetUserById(idKorisnika);

                        if (user == null)
                        {
                            throw new Exception("Грешка: Корисник није враћен!");
                        }

                        Console.WriteLine($"Нађен је корисник:{'\n'}Име: {user.FirstName}" +
                            $", Презиме: {user.LastName},Мејл: {user.Email}, Шифра:{user.Password}");

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();

                        break;
                    }
                case "5":
                    {
                        var users = await SqlRepository.GetAllUsers();

                        /*ConsoleTableBuilder
                            .From(users)
                            .WithTitle("КОРИСНИЦИ ", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                            .WithColumn("ИД", "Име", "Презиме", "Мејл", "Шифра")
                            .ExportAndWriteLine();*/
                        foreach (User user in users)
                        {
                            Console.WriteLine(user);
                        }

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Непозната опција. Покушајте поново..(притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Грешка: {ex.Message} {Environment.NewLine} (притисните било који тастер за наставак)");
            Console.ReadKey();
        }
    }
}

async Task CurrencyUseCases()
{
    var goBackRequested = false;
    while (goBackRequested == false)
    {
        try
        {
            ShowCurrencyMenu();
            var currencyOption = Console.ReadLine();
            Console.Clear();
            switch (currencyOption)
            {
                case "0":
                    {
                        goBackRequested = true;
                        break;
                    }
                case "1":
                    {

                        Console.WriteLine("Унесите име валуте:");
                        var name = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите код:");
                        var code = Console.ReadLine() ?? "";

                        var newCurrency = new Currency()
                        {
                            Name = name,
                            Code = code

                        };
                        ValidateCurrencyName(newCurrency);

                        if (string.IsNullOrWhiteSpace(newCurrency.Code))
                            throw new Exception("Морате унети koд валуте");



                        var user = await SqlRepository.CreateCurrency(newCurrency);

                        Console.WriteLine($"Додата нова валута. (притисните било који тастер за наставак)");
                        Console.ReadKey();

                        break;
                    }
                case "2":
                    {
                        Console.Write("Унесите Ид валуте коју желите да измените: ");

                        if (!int.TryParse(Console.ReadLine(), out int updateId))
                            throw new Exception("Погрешно сте унели Ид!");

                        Console.WriteLine("Унесите име валуте:");
                        var name = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите код:");
                        var code = Console.ReadLine() ?? "";

                        var updCurrency = new Currency()
                        {
                            Name = name,
                            Code = code

                        };

                        ValidateCurrencyName(updCurrency);

                        if (string.IsNullOrWhiteSpace(updCurrency.Code))
                            throw new Exception("Морате унети kog   валуте");

                        var currency = await SqlRepository.UpdateCurrency(updateId, updCurrency);

                        Console.WriteLine($"Валута којој је ид {updateId} је успешно измењена. (притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
                case "3":
                    {
                        Console.Write("Унесите ид валуте коју желите да обришете: ");
                        if (!int.TryParse(Console.ReadLine(), out int id))
                        {
                            throw new Exception("Погрешно сте унели Ид!");

                        }
                        var curr = await SqlRepository.DeleteCurrency(id);
                        Console.WriteLine("Валута је успешно обрисана. (притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
                case "4":
                    {
                        Console.Write("Унесите Ид валуте коју желите да прикажете: ");

                        if (!int.TryParse(Console.ReadLine(), out int id))
                        {
                            throw new Exception("Погрешно сте унели Ид!");

                        }
                        var curr = await SqlRepository.GetCurrencyById(id);

                        if (curr == null)
                        {
                            throw new Exception("Валута није враћена!");

                        }


                        Console.WriteLine($"Нађена је валута:{'\n'}Ид: {curr.Id}" +
                           $", Назив: {curr.Name},Код: {curr.Code}");

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();


                        break;
                    }
                case "5":
                    {
                        var currencies = await SqlRepository.GetCurrencies();

                        /*ConsoleTableBuilder
                            .From(currencies)
                            .WithTitle("ВАЛУТЕ ", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                            .WithColumn("ИД", "Име", "Код")
                            .ExportAndWriteLine();*/
                        foreach (Currency curr in currencies)
                        {
                            Console.WriteLine(curr);
                        }

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Непозната опција. Покушајте поново..(притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Грешка: {ex.Message} {Environment.NewLine} (притисните било који тастер за наставак)");
            Console.ReadKey();
        }
    }
}

void ShowUserMenu()
{
    Console.Clear();
    Console.WriteLine("1. Додај");
    Console.WriteLine("2. Ажурирај");
    Console.WriteLine("3. Обриши");
    Console.WriteLine("4. Прикажи једног");
    Console.WriteLine("5. Прикажи све");
    Console.WriteLine("0. Назад");
    Console.Write("Одаберите опцију: ");
}

void ShowCurrencyMenu()
{
    Console.Clear();
    Console.WriteLine("1. Додај");
    Console.WriteLine("2. Ажурирај");
    Console.WriteLine("3. Обриши");
    Console.WriteLine("4. Прикажи једну валуту");
    Console.WriteLine("5. Прикажи све");
    Console.WriteLine("0. Назад");
    Console.Write("Одаберите опцију: ");
}

void ShowTransactionMenu()
{
    Console.Clear();
    Console.WriteLine("1. Додај");
    Console.WriteLine("2. Ажурирај");
    Console.WriteLine("3. Прикажи једну трансакцију");
    Console.WriteLine("4. Прикажи све");
    Console.WriteLine("0. Назад");
    Console.Write("Одаберите опцију: ");
}

async Task TransactionUseCases()
{
    var goBackRequested = false;
    while (goBackRequested == false)
    {
        try
        {
            ShowTransactionMenu();
            var userOption = Console.ReadLine();
            Console.Clear();
            switch (userOption)
            {
                case "0":
                    {
                        goBackRequested = true;
                        break;
                    }
                case "1":
                    {
                        Console.WriteLine("Унесите износ: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                        {
                            throw new Exception("Износ није у добром формату!");
                        }

                        Console.WriteLine("Унесите датум у формату \"dd/MM/yyyy\" ");
                        var date = Console.ReadLine() ?? "";
                        if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                        {
                            throw new Exception("Нисте унели датум у добром формату!");
                        }

                        Console.WriteLine("Унесите Ид рачуна са ког се пребацује новац или null уколико није потребан: ");
                        var p = Console.ReadLine();
                        int? idFromNull;
                        if (p == "null")
                        {
                            idFromNull = null;
                        }
                        else
                        {
                            int idFrom;
                            if (int.TryParse(p, out idFrom) == false)
                            {
                                throw new Exception("IdFrom није у добром формату!");
                            }
                           
                            
                                idFromNull = idFrom;
                            
                        }


                        Console.WriteLine("Унесите Ид рачуна на који се пребацује новац или -1 уколико није потребан: ");
                        var q = Console.ReadLine();
                        int? idToNull;
                        if (q == "-1")
                        {
                            idToNull = null;
                        }
                        else
                        {
                            if (!int.TryParse(q, out int idTo))
                            {
                                throw new Exception("IdTo није у добром формату!");
                            }
                            
                                idToNull = idTo;
                            
                        }

                        var newTrans = new Transaction()
                        {
                            Amount = amount,
                            DateOfRealisation = dt,
                            FromAccountId = idFromNull,
                            ToAccountId = idToNull
                        };



                        var trans = await SqlRepository.CreateTransaction(newTrans);

                        Console.WriteLine($"Додата је нова трансакција: '{trans}'. (притисните било који тастер за наставак)");
                        Console.ReadKey();

                        break;
                    }
                case "2"://napraviti
                    {

                        Console.WriteLine("Унесите Ид трансакције коју желите да измените:");
                        var updId = Console.ReadLine();

                        if (!int.TryParse(updId, out int updateId))
                            throw new Exception("Погрешно сте унели Ид!");
                        if((await SqlRepository.GetTransactionById(updateId)) == null)
                        {
                            throw new Exception("Трансакција не постоји!");
                        }

                        Console.WriteLine("Унесите износ: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                        {
                            throw new Exception("Износ није у добром формату!");
                        }

                        Console.WriteLine("Унесите датум у формату \"dd/MM/yyyy\" ");
                        var date = Console.ReadLine() ?? "";
                        if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                        {
                            throw new Exception("Нисте унели датум у добром формату!");
                        }

                        Console.WriteLine("Унесите Ид рачуна са ког се пребацује новац или null уколико није потербан: ");
                        var p = Console.ReadLine();
                        int? idFromNull;
                        if (p == "null")
                        {
                            idFromNull = null;
                        }
                        else
                        {
                            int idFrom;
                            if (int.TryParse(p, out idFrom) == false)
                            {
                                throw new Exception("IdFrom није у добром формату!");
                            }
                            idFromNull = idFrom;
                        }


                        Console.WriteLine("Унесите Ид рачуна на који се пребацује новац или null уколико није потербан: ");
                        var q = Console.ReadLine();
                        int? idToNull;
                        if (q == "null")
                        {
                            idToNull = null;
                        }
                        else
                        {
                            if (!int.TryParse(q, out int idTo))
                            {
                                throw new Exception("IdTo није у добром формату!");
                            }

                            idToNull = idTo;
                        }

                        var newTrans = new Transaction()
                        {
                            Amount = amount,
                            DateOfRealisation = dt,
                            FromAccountId = idFromNull,
                            ToAccountId = idToNull
                        };



                        var trans = await SqlRepository.UpdateTransaction(updateId,newTrans);

                        Console.WriteLine($"Трансакција је успешно измењена: '{trans}'. (притисните било који тастер за наставак)");
                        Console.ReadKey();

                        break;
                    }
                case "3":
                    {

                        Console.Write("Унесите ИД трансакције коју желите да прикажете: ");
                        if (!int.TryParse(Console.ReadLine(), out int idTrans))
                        {
                            throw new Exception("Погрешно сте унели Ид!");
                        }
                        var trans = await SqlRepository.GetTransactionById(idTrans);

                        if (trans == null)
                        {
                            throw new Exception("Грешка: Трансакција није враћена!");
                        }

                        Console.WriteLine($"Нађена је трансакција:{'\n'}Ид  : {trans.Id}" +
                            $", Износ: {trans.Amount}, Датум: {trans.DateOfRealisation}, Од:{trans.FromAccountId}, За: {trans.ToAccountId}");

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();

                        break;
                    }
                case "4":
                    {
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Непозната опција. Покушајте поново..(притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Грешка: {ex.Message} {Environment.NewLine} (притисните било који тастер за наставак)");
            Console.ReadKey();
        }

    }
}

void ShowAccountMenu()
{
    Console.Clear();
    Console.WriteLine("1. Додај");
    Console.WriteLine("2. Ажурирај");
    Console.WriteLine("3. Прикажи један рачун");
    Console.WriteLine("4. Прикажи све");
    Console.WriteLine("0. Назад");
    Console.Write("Одаберите опцију: ");
}

async Task AccountUseCases()
{
    var goBackRequested = false;
    while (goBackRequested == false)
    {
        try
        {
            ShowAccountMenu();
            var userOption = Console.ReadLine();
            Console.Clear();
            switch (userOption)
            {
                case "0":
                    {
                        goBackRequested = true;
                        break;
                    }
                case "1":
                    {
                        Console.WriteLine("Унесите балaнс:");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
                        {
                            throw new Exception("Баланс није у добром формату!");
                        }

                        Console.WriteLine("Унесите статус (Active или Inactive): ");
                        var status = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите број рачуна:");
                        var number = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите userID:");
                        if (!int.TryParse(Console.ReadLine(), out int userId))
                        {
                            throw new Exception("UserId није у добром формату!");

                        }
                        if((await SqlRepository.GetUserById(userId)) == null)
                        {
                            throw new Exception("Корисник не постоји!");
                        }
                        
                        Console.WriteLine("Унесите currencyId:");
                        if (!int.TryParse(Console.ReadLine(), out int currencyId))
                        {
                            throw new Exception("Currency није у добром формату!");
                        }
                        if ((await SqlRepository.GetCurrencyById(currencyId)) == null)
                        {
                            throw new Exception("Валута не постоји!");
                        }
                        var newAccount = new Account()
                        {
                            Balance = balance,
                            Number = number,
                            UserId = userId,
                            CurrencyId = currencyId
                        };

                        if (status == "Active")
                        {
                            newAccount.Status = Status.Active;
                        }
                        else if (status == "NotAactive")
                        {
                            newAccount.Status = Status.NotActive;
                        }
                        else
                        {
                            throw new Exception("Cтатус није у добром формату!");
                        }

                        ValidateAccountNumber(newAccount);

                        var account = await SqlRepository.CreateAccount(newAccount);
                        Console.WriteLine($"Додат нови рачун: '{account}'. (притисните било који тастер за наставак)");
                        Console.ReadKey();

                        break;
                    }
                case "2":
                    {
                        Console.Write("Унесите Ид рачуна који желите да измените: ");

                        if (!int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            throw new Exception("Погрешан формат ИД-а!");

                        }
                        Account acc = await SqlRepository.GetAccountById(updateId);
                        if (acc == null)
                        {
                            throw new Exception("Рачун не постоји!");
                        }
                        Console.WriteLine("Унесите балaнс:");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
                            throw new Exception("Баланс није у добром формату!");

                        Console.WriteLine("Унесите статус (Active или NotActive: ");
                        var status = Console.ReadLine() ?? "";

                        Console.WriteLine("Унесите број рачуна:");
                        var number = Console.ReadLine() ?? "";

                        var updAccount = new Account()
                        {
                            Balance = balance,
                            Number = number,
                            UserId=acc.UserId,
                            CurrencyId=acc.CurrencyId


                        };

                        if (status == "Active")
                        {
                            updAccount.Status = Status.Active;
                        }
                        else if (status == "Inactive")
                        {
                            updAccount.Status = Status.NotActive;
                        }
                        else
                        {
                            throw new Exception("Cтатус није у добром формату!");
                        }

                        ValidateAccountNumber(updAccount);

                        var account = await SqlRepository.UpdateAccount(updateId, updAccount);

                        Console.WriteLine($"Рачун коме је ид {updateId} је успешно измењен. (притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
                case "3":
                    {

                        Console.Write("Унесите ИД рачуна који желите да прикажете: ");
                        if (!int.TryParse(Console.ReadLine(), out int idAcc))
                        {
                            throw new Exception("Погрешно сте унели Ид!");
                        }
                        var account = await SqlRepository.GetAccountById(idAcc);
                        if (account == null)
                        {
                            throw new Exception("Рачун није враћен!");

                        }


                        Console.WriteLine($"Нађен је рачун:{'\n'}Ид: {account.Id}" +
                            $", Стање: {account.Balance},Статус: {account.Status}, Број:{account.Number}");

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();


                        break;
                    }
                case "4":
                    {
                        var accounts = await SqlRepository.GetAllAccounts();

                        /*ConsoleTableBuilder
                            .From(accounts)
                            .WithTitle("РАЧУНИ ", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                            .WithColumn("ИД", "Стање", "Статус", "Број", "Корисник", "Валута")
                            .ExportAndWriteLine();*/
                        foreach(Account acc in accounts)
                        {
                            Console.WriteLine(acc);
                        }

                        Console.WriteLine("Притисните било који тастер за наставак...");
                        Console.ReadKey();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Непозната опција. Покушајте поново..(притисните било који тастер за наставак)");
                        Console.ReadKey();
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Грешка: {ex.Message} {Environment.NewLine} (притисните било који тастер за наставак)");
            Console.ReadKey();
        }

    }

}
