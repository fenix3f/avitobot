using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Passport;
using Telegram.Bot.Types.ReplyMarkups;
using avitomisisDB;
using Newtonsoft.Json.Serialization;

class Program
    {
        // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
        private static ITelegramBotClient _botClient;

       

        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions _receiverOptions;

        private static string connectionString = ConfigurationManager.ConnectionStrings["GoodsDB"].ConnectionString;
        static async Task Main()
        {

            _botClient = new TelegramBotClient("1523366550:AAF2zBjJRWmxLfR4lH1a5vQzFGaq9lHt17g"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
            _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
            {
                AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
                {
                UpdateType.Message,
                UpdateType.CallbackQuery// Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
                // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
                // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
                ThrowPendingUpdates = true,

            };

            using var cts = new CancellationTokenSource();

            // UpdateHander - обработчик приходящих Update`ов
            // ErrorHandler - обработчик ошибок, связанных с Bot API
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

            var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }

        static Boolean filter_buy = false;
        static Boolean filter_sell = false;
        static Boolean sell_choose = false;
        static Boolean cat = false, fil = false, pho = false, nm = false, sl = false, dp = false,ct = false, pho_add = false;
        static string name = " ";
    static string tg_name = null, desc = null, photo_load = null;
    static double sell = 0;
    static string[] tag = new string[3] { null, null, null };
    static string photografi;
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            
            
            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // 0 переменные для БД
                

            //1
            const string Buyer = "Я покупатель";
                const string Solver = "Я продавец";
                const string Back = "Назад", Back2 = "Назад)";
                const string Yes = "Да";
                const string No = "Нет";
                //2 for buyers
                const string category = "Вид товара"
                    , fillial_obschagi = "Общага",
                    photo_exist = "Наличие Фото",
                    next_step = "Дальше",
                    ch_name = "Имя",cost = "Цена", tg_tag = "Контакты", description = "Описание", photo_add = "Добавить фото";
                //3 for solvers
                const string my_offers = "Мои объявления", create = "Создать объявление";


                //4 categories
                const string tecnic1 = "Техника", bitov = "Бытовое";
                

                //Reply кнопки покупателя
                ReplyKeyboardMarkup ChooseFate = new(new[]
            {
         new KeyboardButton[] {Buyer, Solver},
    })
                {
                    ResizeKeyboard = true,
                };
                ReplyKeyboardMarkup filters = new(new[]
            {
         new KeyboardButton[] { category,  fillial_obschagi, photo_exist},
         new KeyboardButton[] { ch_name,  next_step },
    })
                {
                    ResizeKeyboard = true,
                };
            ReplyKeyboardMarkup filters2 = new(new[]
                      {
         new KeyboardButton[] { category,  fillial_obschagi, photo_add},
         new KeyboardButton[] { ch_name,  next_step },
         new KeyboardButton[] { cost, tg_tag, description },
    })
            {
                ResizeKeyboard = true,
            };

            ReplyKeyboardMarkup back = new(new[]
            {
            new KeyboardButton[] { Back },
        })
            {
                ResizeKeyboard = true,
            };

            ReplyKeyboardMarkup back2 = new(new[]
            {
            new KeyboardButton[] { Back2 },
        })
             {
                    ResizeKeyboard = true,
                };

                //Reply кнопки продавца
                ReplyKeyboardMarkup main_menu_solver = new(new[]
            {
            new KeyboardButton[] { my_offers },
            new KeyboardButton[] { create }
        })
                {
                    ResizeKeyboard = true,
                };



                //Inline кнопки
                InlineKeyboardMarkup categories = new(new[]
                    {
         new[] {
             InlineKeyboardButton.WithCallbackData(text: tecnic1, callbackData: tecnic1),
             InlineKeyboardButton.WithCallbackData(text: bitov, callbackData: bitov)},
    });

  
                InlineKeyboardMarkup photos = new(new[]
                        {
         new[] {
             InlineKeyboardButton.WithCallbackData(text: Yes, callbackData: Yes),
             InlineKeyboardButton.WithCallbackData(text: No, callbackData: No)},
    });
                InlineKeyboardMarkup fillial = new(new[]
                        {
         new[] {
             InlineKeyboardButton.WithCallbackData(text: "Г-1", callbackData: "Г-1"),
             InlineKeyboardButton.WithCallbackData(text: "Г-2", callbackData: "Г-2"),
             InlineKeyboardButton.WithCallbackData(text: "Дом-коммуна", callbackData: "Дом-коммуна"),
             InlineKeyboardButton.WithCallbackData(text: "ДСГ", callbackData: "ДСГ"),
             InlineKeyboardButton.WithCallbackData(text: "М-1", callbackData: "М-1"),
             InlineKeyboardButton.WithCallbackData(text: "М-2", callbackData: "М-2"),
             InlineKeyboardButton.WithCallbackData(text: "М-3", callbackData: "М-3"),
             InlineKeyboardButton.WithCallbackData(text: "М-4", callbackData: "М-4")},
    });

                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            // Эта переменная будет содержать в себе все связанное с сообщениями
                            var message = update.Message;



                            //То, что вводит пользователь
                            var messageText = message.Text;

                            // From - это от кого пришло сообщение (или любой другой Update)
                            var user = message.From;

                            // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            // Chat - содержит всю информацию о чате
                            var chat = message.Chat;

                            //Номер чата
                            var chatId = chat.Id;

                            //Костыль для имени
                            if (nm)
                            {
                                name = messageText;
                                nm = false;

                                return;
                            }
                            if (sl)
                            {
                            try
                                {
                                sell = Convert.ToDouble(messageText);
                                
                            }
                            catch (Exception ex)
                            {

                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Используй только цифры! (Повторно нажмите 'Цена')");
                            }
                            sl = false;
                            return;
                            }
                            if (dp)
                            {
                                desc =  messageText;
                                dp = false;

                                return;
                            }
                        if (pho_add)
                        {
                            photografi = messageText;
                            pho_add = false;

                            return;
                        }
                        if (ct)
                            {
                                tg_name = messageText;
                                ct = false;

                                return;
                            }

                        // Добавляем проверку на тип Message
                        switch (message.Type)
                            {
                                // Тут понятно, текстовый тип
                                case MessageType.Text:
                                    {
                                        // тут обрабатываем команду /start, остальные аналогично
                                        if (messageText == "/start")
                                        {

                                            Message sentMessage = await botClient.SendTextMessageAsync(
                                                chatId: chatId,
                                                text: "Здравствуй!! Ты в боте ❤❤❤AVITO@MISIS❤❤❤, чтобы продолжить, выбери кто ты:",
                                                replyMarkup: ChooseFate,
                                                cancellationToken: cancellationToken);
                                            filter_buy = false;
                                            filter_sell = false;
                                            nm = false;
                                            dp = false;
                                            sl = false;
                                        sell_choose = false;
                                            return;

                                        }
                                        if (messageText == "Я покупатель")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chatId: chatId,
                                                text: "Отлично! Выбери фильтры:",
                                                replyMarkup: filters,
                                                cancellationToken: cancellationToken);
                                            filter_buy = true;
                                        }
                                    if (filter_buy || filter_sell)
                                    {
                                        switch (messageText)
                                        {
                                            // Тут понятно, текстовый тип
                                            case category:
                                                {

                                                    await botClient.SendTextMessageAsync(
                                                    chatId: chatId,
                                                    text: "Выбери категорию товара:",
                                                    replyMarkup: categories);
                                                    cat = true;
                                                    return;

                                                }
                                            case Back:
                                                {
                                                    Message sentMessage = await botClient.SendTextMessageAsync(
                                                    chatId: chatId,
                                                    text: "Выбери фильтры:",
                                                    replyMarkup: filters,

                                                    cancellationToken: cancellationToken);
                                                    nm = false;
                                                    return;

                                                }
                                            
                                            case fillial_obschagi:
                                                {
                                                    Message sentMessage = await botClient.SendTextMessageAsync(
                                                    chatId: chatId,
                                                    text: "Выбери филлиал общаги:",
                                                    replyMarkup: fillial,
                                                    cancellationToken: cancellationToken);
                                                    fil = true;
                                                    return;
                                                }
                                            case photo_exist:
                                                {
                                                    
                                                    Message sentMessage = await botClient.SendTextMessageAsync(
                                                       chatId: chatId,
                                                       text: "Нужно ли фото?:",
                                                       replyMarkup: photos,
                                                       cancellationToken: cancellationToken);

                                                    pho = true;
                                                    return;
                                                }
                                            case next_step:
                                                {
                                                    if (filter_buy)
                                                    {
                                                        Message sentMessage = await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        text: $"Вот, что я смог подобрать для вас:",
                                                        replyMarkup: back,
                                                        cancellationToken: cancellationToken);
                                                        List<int> goodID = DB.FindGoodsByTags(tag);
                                                        for (int i=0; i<goodID.Count; i++)
                                                        {
                                                            string goodTags = "";
                                                            string[] TagsArray = DB.GetTagsArrayForGood(goodID[i]);
                                                            for (int j=0; j<TagsArray.Length; j++)
                                                            {
                                                                goodTags =goodTags+TagsArray[j]+" ";
                                                            }
                                                            object[] goodInfo = DB.GetGoodById(goodID[i]);
                                                            
                                                                await botClient.SendTextMessageAsync(
                                                                chatId: chatId,
                                                                text: $"Имя: {goodInfo[1]} \n" +
                                                                      $"\n" +
                                                                      $"Описание: {goodInfo[2]} \n " +
                                                                      $"\n" +
                                                                      $"Цена: {goodInfo[3]:f2} \n" +
                                                                      $"\n" +
                                                                      $"Контакты: {goodInfo[4]} \n" +
                                                                      $"\n" +
                                                                      $"Тэги: {goodTags} \n",
                                                                replyMarkup: back,
                                                                cancellationToken: cancellationToken);
                                                                
                                                            

                                                        }
                                                    }
                                                    if (filter_sell)
                                                    {
                                                        Message sentMessage = await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        text: $"Ваше объявление сохранено! {name}",
                                                        replyMarkup: back2,
                                                        cancellationToken: cancellationToken);
                                                        int LastId = DB.CreateGood(name, desc, sell, tg_name, tag,photografi) ;
                                                        object[] LastGoodInfo = DB.GetGoodById(LastId);
                                                        string goodTags = "";
                                                        string[] TagsArray = DB.GetTagsArrayForGood(LastId);
                                                        for (int j = 0; j < TagsArray.Length; j++)
                                                        {
                                                            goodTags = goodTags + TagsArray[j] + " ";
                                                        }
                                                        for (int i = 1; i < LastGoodInfo.Count(); i++)
                                                        {
                                                            Console.WriteLine(LastGoodInfo[i]);
                                                        }
                                                            await botClient.SendTextMessageAsync(
                                                            chatId: chatId,
                                                            text: $"Имя: {LastGoodInfo[1]} \n" +
                                                                  $"\n" +
                                                                  $"Описание: {LastGoodInfo[2]} \n " +
                                                                  $"\n" +
                                                                  $"Цена: {LastGoodInfo[3]:f2} \n" +
                                                                  $"\n" +
                                                                  $"Контакты: {LastGoodInfo[4]} \n" +
                                                                  $"\n" +
                                                                  $"Тэги: {goodTags} \n",
                                                            replyMarkup: back2,
                                                            cancellationToken: cancellationToken);
                                                        name = null;
                                                        tg_name = null; desc = null;
                                                        sell = 0;
                                                        tag[0] = null;
                                                        tag[1] = null;
                                                        tag[2] = null;
                                                    }
                                                    return;
                                                }
                                            case ch_name:

                                                {
                                                    Message sentMessage = await botClient.SendTextMessageAsync(
                                                    chatId: chatId,
                                                    text: "Введи название товара:",
                                                    cancellationToken: cancellationToken);
                                                    nm = true;
                                                    
                                                    return;
                                                }
                                            case Back2:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        text: "Давайте создадим еще одно объявление!!)",
                                                        replyMarkup: filters2,
                                                        cancellationToken: cancellationToken);
                                                    filter_sell = true;
                                                    return;
                                                }
                                            case cost:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                          chatId: chatId,
                                                          text: "Укажите цену товара:",
                                                          replyMarkup: filters2,
                                                          cancellationToken: cancellationToken);
                                                    sl = true;
                                                    return;
                                                }
                                            case description:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                          chatId: chatId,
                                                          text: "Укажите описание товара:",
                                                          replyMarkup: filters2,
                                                          cancellationToken: cancellationToken);
                                                    dp = true;
                                                    return;
                                                }
                                            case photo_add:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                          chatId: chatId,
                                                          text: "Скиньте фоточку;)",
                                                          replyMarkup: filters2,
                                                          cancellationToken: cancellationToken);
                                                    pho_add = true;
                                                    return;
                                                }
                                            case tg_tag:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                          chatId: chatId,
                                                          text: "Напиши типа телеграмм ник с собакой)))))))))))",
                                                          replyMarkup: filters2,
                                                          cancellationToken: cancellationToken);
                                                    ct = true;
                                                    return;
                                                }
                                        }

                                    }

                                        if (messageText == "Я продавец")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chatId: chatId,
                                                text: "Отлично! Выбери действие:",
                                                replyMarkup: main_menu_solver,
                                                cancellationToken: cancellationToken);
                                        sell_choose = true;
                                        }
                                    if (sell_choose)
                                    {
                                        switch (messageText)
                                        {
                                            case my_offers:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        text: "Ваши объявления:",
                                                        cancellationToken: cancellationToken);
                                                    return;
                                                }
                                            case create:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        text: "Давайте создадим новое объявление!!)",
                                                        replyMarkup: filters2,
                                                        cancellationToken: cancellationToken);
                                                    filter_sell = true;
                                                    return;
                                                }
                                            
                                        }
                                    }




                                    }
                                    return;
                                // Добавил default , чтобы показать вам разницу типов Message
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!");
                                        return;
                                    }
                            }


                        }

                    case UpdateType.CallbackQuery:
                        {
                            // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
                            var callbackQuery = update.CallbackQuery;

                            // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
                            var user = callbackQuery.From;

                            // Выводим на экран нажатие кнопки
                            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                            // Вот тут нужно уже быть немножко внимательным и не путаться!
                            // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
                            // кнопка привязана к сообщению, то мы берем информацию от сообщения.
                            var chat = callbackQuery.Message.Chat;
                            if (cat)
                            {
                                // Добавляем блок switch для проверки кнопок
                                switch (callbackQuery.Data)
                                {
                                    // Data - это придуманный нами id кнопки, мы его указывали в параметре
                                    // callbackData при создании кнопок. У меня это button1, button2 и button3

                                    case tecnic1:
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                       
                                        tag[0] = callbackQuery.Data;
                                            return;
                                        }

                                    case bitov:
                                        {
                                            // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку


                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[0] = callbackQuery.Data;
                                        return;
                                        }
                                }
                                cat = false;
                            }
                           
                            if (pho)
                            {
                                switch (callbackQuery.Data)
                                {
                                    // Data - это придуманный нами id кнопки, мы его указывали в параметре
                                    // callbackData при создании кнопок. У меня это button1, button2 и button3

                                    case Yes:
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");

                                            return;
                                        }

                                    case No:
                                        {
                                            // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку


                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");

                                            return;
                                        }
                                }
                                pho = false;
                            }
                            if (fil)
                            {
                                switch (callbackQuery.Data)
                                {
                                    // Data - это придуманный нами id кнопки, мы его указывали в параметре
                                    // callbackData при создании кнопок. У меня это button1, button2 и button3

                                    case "Г-1":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                            return;
                                        }

                                    case "Г-2":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                    case "Дом-коммуна":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                    case "ДСГ":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                    case "М-1":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                    case "М-2":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                    case "М-3":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                    case "М-4":
                                        {
                                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                            await botClient.DeleteMessageAsync(
                                                chat.Id,
                                                messageId: callbackQuery.Message.MessageId,
                                                cancellationToken: cancellationToken);
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Вы нажали на {callbackQuery.Data}");
                                        tag[2] = callbackQuery.Data;
                                        return;
                                        }
                                }
                                fil = false;
                            }

                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
