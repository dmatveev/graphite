class GraphiteApplication {
    public static void Main () {
        Core.Vertex v = new Core.Vertex (id: 10);
        for (int i = 0; i < 5; i++) {
            v.Connect (new Core.Vertex (id: i));
        }
        System.Console.WriteLine (v);
    }
}