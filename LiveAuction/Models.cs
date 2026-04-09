namespace LiveAuction;

public class Lot
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Emoji { get; set; } = "";
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public string? TopBidder { get; set; }  // null якщо ще ніхто не ставив
}

public class BidRequest
{
    public string Bidder { get; set; } = "";
    public decimal Amount { get; set; }
}
