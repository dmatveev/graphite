using System;
using System.IO;
using System.Collections.Generic;

class Vertex {
    protected int _id;
    protected List<Vertex> _related;

    public Vertex (int id = 0) {
        _id = id;
        _related = new List<Vertex>();
    }

    public void connect (Vertex v) {
        if (!_related.Contains (v)) {
            _related.Add (v);
        }
    }

    public Vertex[] related () {
        return _related.ToArray();
    }

    public static implicit operator string (Vertex v) {
        StringWriter writer = new StringWriter ();
        writer.Write ("Vertex #{0}: ", v._id);

        foreach (Vertex each in v._related) {
            writer.Write ("#{0} ", each._id);
        }
        return writer.ToString();
    }
}