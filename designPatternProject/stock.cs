using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designPatternProject
{
    internal class Stock
    {
        public List<Robot> robots { get; set; }
        public List<Piece> pieces { get; set; }

      

        public Stock(List<Piece> pieces, List<Robot> robots)
        {
            this.pieces = pieces ?? new List<Piece>();
            this.robots = robots ?? new List<Robot>();
        }


        public void AfficherStock()
        {
            Console.WriteLine("Stock des robots :");
            foreach (var robot in robots)
            {
                Console.WriteLine($"Robot : {robot.name}, Quantité : {robot.quantite}");
               
            }
            Console.WriteLine("\nStock des pièces :");
            foreach (var piece in pieces)
            {
                Console.WriteLine($"Pièce : {piece.name}, Quantité : {piece.quantite}");
            }
        }

        public void pieceParRobot(string robotType ,int qtt)
        {
            var robot = robots.FirstOrDefault(r => r.name == robotType);
            if (robot != null)
            {
                Console.WriteLine($"Robot : {robot.name}, Quantité : {qtt}");
                robot.Affichepieces(qtt);
            }
            else
            {
                Console.WriteLine($"Robot de type {robotType} non trouvé.");
            }
        }


      







    }
}
