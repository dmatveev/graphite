class GraphiteApplication {
    public static void Main () {
        Vertex v = new Vertex (id: 10);
        for (int i = 0; i < 5; i++) {
            v.connect (new Vertex (id: i));
        }
        System.Console.WriteLine (v);
    }
}