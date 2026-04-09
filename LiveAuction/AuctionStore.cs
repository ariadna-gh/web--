namespace LiveAuction;

public static class AuctionStore
{
    public static List<Lot> Lots { get; } = new()
    {
        new Lot { Id = 1, Title = "Картина Моне",     Emoji = "🖼️",  StartPrice = 1000, CurrentPrice = 1000 },
        new Lot { Id = 2, Title = "Антична ваза",     Emoji = "🏺",  StartPrice = 500,  CurrentPrice = 500  },
        new Lot { Id = 3, Title = "Годинник 19 ст.",  Emoji = "🕰️",  StartPrice = 750,  CurrentPrice = 750  },
    };
}
