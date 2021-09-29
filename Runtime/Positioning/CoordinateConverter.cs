using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JelleKUL.MeshAlignment
{
    // the different coordinates to have your measurements in
    public enum CoordinateSystem { Lambert72, Lambert2008, Spherical }

    public class CoordinateConverter : MonoBehaviour
    {

        //todo add more coordinates
        /// <summary>
        /// Convert a position to another coordinate system
        /// </summary>
        /// <param name="input">the position in (x,y,z) or (long, lat, height)</param>
        /// <param name="fromCoordinate">the coordinatesystem the input is in</param>
        /// <param name="toCoordinate">the coordinate system the output has to be</param>
        /// <returns>converted coordinates in the toCoordinate system</returns>
        public static Vector3 ConvertCoordinates(Vector3 input, CoordinateSystem fromCoordinate, CoordinateSystem toCoordinate)
        {
            switch (fromCoordinate)
            {
                case CoordinateSystem.Spherical:
                    switch (toCoordinate)
                    {
                        case CoordinateSystem.Lambert72:
                            Vector2 coordinates = SphereToLambert72(input.x, input.y);
                            return new Vector3(coordinates.x, input.z, coordinates.y); // unity y is up and z is forward

                        //todo add more cases

                        default:
                            break;
                    }
                    break;

                //todo add more cases

                default:
                    break;
            }

            return input;
        }


        /// <summary>
        /// Concert spherical coortinates to lambert72 coordinates
        /// http://zoologie.umons.ac.be/tc/algorithms.aspx#M2
        /// </summary>
        /// <param name="lng">the longitude in decimal belgium Datum</param>
        /// <param name="lat">the latitude in decimal belgium Datum</param>
        /// <returns>the lambert72 x & y coordinates</returns>
        public static Vector2 SphereToLambert72(float lng, float lat)
        {

            float LongRef = 0.076042943f;      //=4°21'24"983
            float bLamb = 6378388 * (1 - (1 / 297.0f));
            float aCarre = Mathf.Pow(6378388,2);
            float eCarre = (aCarre - bLamb * bLamb) / aCarre;
            float KLamb = 11565915.812935f;
            float nLamb = 0.7716421928f;

            float eLamb = Mathf.Sqrt(eCarre);
            float eSur2 = eLamb / 2f;

            //conversion to radians
            lat *= (Mathf.PI / 180f);
            lng *= (Mathf.PI / 180f);


            float eSinLatitude = eLamb * Mathf.Sin(lat);
            float TanZDemi = (Mathf.Tan((Mathf.PI / 4f) - (lat / 2f))) * Mathf.Pow((1 + eSinLatitude) / (1 - eSinLatitude), eSur2);

            float RLamb = KLamb * Mathf.Pow(TanZDemi, nLamb);

            float Teta = nLamb * (lng - LongRef);

            float x = 150000  + 0.01256f + RLamb* Mathf.Sin(Teta - 0.000142043f);
            float y = 5400000 + 88.4378f - RLamb* Mathf.Cos(Teta - 0.000142043f);

            return new Vector2(x, y);
        }
    }
}

