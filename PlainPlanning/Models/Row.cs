using System.Drawing;

namespace PlanePlanning.Models
{
    public class Row
    {
        public int Id { get; set; }
        public string[][] seats { get; set; } = new string[2][];

        public Row(int left, int right)
        {
            seats[0] = new string[left];
            seats[1] = new string[right];
        }

        public int getFreeSeats()
        {
            int freeSeats = 0;
            foreach (string seat in seats[0])
            {
                if (string.IsNullOrEmpty(seat))
                {
                    freeSeats++;
                }
            }
            foreach (string seat in seats[1])
            {
                if (string.IsNullOrEmpty(seat))
                {
                    freeSeats++;
                }
            }
            return freeSeats;
        }

        public bool hasFreeSeats(int side, int requestedFreeSeats)
        {
            int freeSeats = 0;
            foreach (string seat in seats[side])
            {
                if (string.IsNullOrEmpty(seat))
                {
                    freeSeats++;
                }
            }
            return freeSeats >= requestedFreeSeats;
        }

        // we have already check that the passangers can fit here so there will no errors
        public void assignSeats(int side, List<string> passengersIds)
        {
            int firstFreeSeat = 0;
            for (int i = 0; i < seats[side].Length; i++)
            {
                if (string.IsNullOrEmpty(seats[side][i]))
                {
                    firstFreeSeat = i;
                    break;
                }
            }

            for (int i = firstFreeSeat; i < passengersIds.Count; i++)
            {
                seats[side][i] = passengersIds[i];
            }
        }

        public void assignSeat(string passenger)
        {
            for (int i = 0; i < seats.Length; i++)
            {
                for (int j = 0; j < seats[i].Length; j++)
                {
                    if (string.IsNullOrEmpty(seats[i][j]))
                    {
                        seats[i][j] = passenger;
                        break;
                    }
                }
            }

        }
    }
}
