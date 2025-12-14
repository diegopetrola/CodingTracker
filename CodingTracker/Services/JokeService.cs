namespace CodingTracker.Services;

public class JokeService
{
    static List<Joke> jokes = [];
    private Random rand = new();

    public JokeService()
    {
        // I am not making a database for the jokes! XD
        // Credit goes to Google AI and github.com/wesbos/dad-jokes
        // No harm or prejudice intended.
        jokes.Add(new Joke
        {
            Question = "Where do Task functions wash their hands?",
            Answer = "At async."
        });
        jokes.Add(new Joke
        {
            Question = "I'm starting a band called HTML Encoder",
            Answer = "Looking to buy a guitar &amp;"
        });
        jokes.Add(new Joke
        {
            Question = "Why did the functions stop calling each other?",
            Answer = "Because they had constant arguments."
        });
        jokes.Add(new Joke
        {
            Question = "Why do Java programmers wear glasses?",
            Answer = "Because they can't C#."
        });
        jokes.Add(new Joke
        {
            Question = "How many developers does it take to change a lightbulb?",
            Answer = "None, that's a hardware problem."
        });
        jokes.Add(new Joke
        {
            Question = "A software tester walks into a bar.",
            Answer = "Runs into a bar. Crawls into a bar. Dances into a bar... "
        });
        jokes.Add(new Joke
        {
            Question = "I've got a really good UDP joke to tell you...",
            Answer = "... but I don't know if you'll get it."
        });
        jokes.Add(new Joke
        {
            Question = "What's the second movie about a database engineer called?",
            Answer = "The SQL."
        });
        jokes.Add(new Joke
        {
            Question = "I never tell the same joke twice",
            Answer = "...I have a DRY sense of humor."
        });
        jokes.Add(new Joke
        {
            Question = "Why was the computer freezing?",
            Answer = "It left its Windows open!"
        });
        jokes.Add(new Joke
        {
            Question = "How do programming pirates pass method parameters?",
            Answer = "Varrrrarrrgs!"
        });
        jokes.Add(new Joke
        {
            Question = "How did pirates collaborate before computers?",
            Answer = "They used pier to pier networking."
        });
        jokes.Add(new Joke
        {
            Question = "You know what the best thing about booleans is?",
            Answer = "Even if you are wrong, you are only off by a bit."
        });
        jokes.Add(new Joke
        {
            Question = "A SQL developer walked into a NoSQL bar.",
            Answer = "They left because they couldn't find a table."
        });
        jokes.Add(new Joke
        {
            Question = "What did the Class say in court when put on trial?",
            Answer = "I strongly object!"
        });
        jokes.Add(new Joke
        {
            Question = "Why do assembly programmers need to know how to swim?",
            Answer = "Because they work below C level."
        });
        jokes.Add(new Joke
        {
            Question = "Why are machine learning models so fit?",
            Answer = "Because they do a lot of weight training!"
        });
        jokes.Add(new Joke
        {
            Question = "What are clouds made of?",
            Answer = "Mostly linux servers."
        });
        jokes.Add(new Joke
        {
            Question = "Why couldn't the React component understand the joke?",
            Answer = "Because it didn't get the context."
        });
        jokes.Add(new Joke
        {
            Question = "Why shouldn't you trust Matlab developers?",
            Answer = "Because they're always plotting something."
        });
    }
    public Joke GetJoke()
    {
        return jokes.ElementAt(rand.Next(jokes.Count));
    }

}


public class Joke
{
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
}
