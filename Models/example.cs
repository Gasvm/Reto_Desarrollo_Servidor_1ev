using Models;

public class Bebida : Producto {

    public bool EsAlcoholica {get;set;}

public Bebida(string nombre, double precio, bool esAlcoholica): base(nombre, precio) {
   EsAlcoholica = esAlcoholica;
}
public Bebida(){}

    public override void MostrarDetalles() {
        string llevaAlcohol = EsAlcoholica ? "Sí" : "No";
        Console.WriteLine($"Bebida: {Nombre}, Precio {Precio:C}, ¿Es alcohólica? {llevaAlcohol} ");
    }
}