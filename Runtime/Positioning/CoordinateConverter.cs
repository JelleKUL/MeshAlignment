using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JelleKUL.MeshAlignment
{
    // todo add more conversion methods
    // the different coordinates to have your measurements in
    public enum CoordinateSystem { Lambert72, Lambert2008, WGS84 }

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
                case CoordinateSystem.WGS84:
                    switch (toCoordinate)
                    {
                        case CoordinateSystem.Lambert72:
                            Vector3 belSphere = WSG48ToBD72(input.x, input.y, input.z);
                            Vector2 coordinates = BD72ToLambert72(belSphere.x, belSphere.y);
                            return new Vector3(coordinates.x, belSphere.z, coordinates.y); // unity y is up and z is forward

                        case CoordinateSystem.WGS84:
                            return input;
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
        public static Vector2 BD72ToLambert72(float lng, float lat)
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
            lat *= Mathf.Deg2Rad;
            lng *= Mathf.Deg2Rad; ;


            float eSinLatitude = eLamb * Mathf.Sin(lat);
            float TanZDemi = (Mathf.Tan((Mathf.PI / 4f) - (lat / 2f))) * Mathf.Pow((1 + eSinLatitude) / (1 - eSinLatitude), eSur2);

            float RLamb = KLamb * Mathf.Pow(TanZDemi, nLamb);

            float Teta = nLamb * (lng - LongRef);

            float x = 150000  + 0.01256f + RLamb* Mathf.Sin(Teta - 0.000142043f);
            float y = 5400000 + 88.4378f - RLamb* Mathf.Cos(Teta - 0.000142043f);

            return new Vector2(x, y);
        }

        public static Vector3 WSG48ToBD72 (float Lng, float Lat, float Alt)
        {
            float LatBel, LngBel;
            float Dh;

            //conversion to radians
            Lat *= Mathf.Deg2Rad;
            Lng *= Mathf.Deg2Rad;


            float SinLat = Mathf.Sin(Lat);
            float SinLng = Mathf.Sin(Lng);
            float CoSinLat = Mathf.Cos(Lat);
            float CoSinLng = Mathf.Cos(Lng);


            float dx = 125.8f;
            float dy = -79.9f;
            float dz = 100.5f;
            float da = 251.0f;
            float df = 0.000014192702f;


            float LWf = 1 / 297f;
            float LWa = 6378388;
            float LWb = (1 - LWf) * LWa;
            float LWe2 = (2 * LWf) - (LWf * LWf);
            float Adb = 1 / (1f - LWf);


            float Rn = LWa / Mathf.Sqrt(1 - LWe2 * SinLat * SinLat);
            float Rm = LWa * (1 - LWe2) / Mathf.Pow((1 - LWe2 * Lat * Lat), 1.5f);


            float DLat = -dx * SinLat * CoSinLng - dy * SinLat * SinLng + dz * CoSinLat;
            DLat += da * (Rn * LWe2 * SinLat * CoSinLat) / LWa;
            DLat += df * (Rm * Adb + Rn / Adb) * SinLat * CoSinLat;
            DLat /= (Rm + Alt);


            float DLng = (-dx * SinLng + dy * CoSinLng) / ((Rn + Alt) * CoSinLat);
            Dh = dx * CoSinLat * CoSinLng + dy * CoSinLat * SinLng + dz * SinLat;
            Dh = Dh - da * LWa / Rn + df * Rn * Lat * Lat / Adb;


            LatBel = ((Lat + DLat) * 180) / Mathf.PI;
            LngBel = ((Lng + DLng) * 180) / Mathf.PI;

            return new Vector3(LngBel, LatBel, Alt + Dh);
        } 
    }
}

