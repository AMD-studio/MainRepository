using UnityEngine;


namespace Enviroment
{
    internal class SoundArea : MonoBehaviour
    {
        public float soundRadius = 10f; // Радиус звука
        public float fadeDistance = 20f; // Расстояние, на котором звук угасает
        public int numberOfPoints = 10; // Количество точек в кубе
        public Transform soundPointPrefab; // Префаб для точек звукового источника

        private void Start()
        {
            CreateSoundPoints();
        }

        private void CreateSoundPoints()
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                float randomX = Random.Range(-soundRadius, soundRadius);
                float randomY = Random.Range(-soundRadius, soundRadius);
                float randomZ = Random.Range(-soundRadius, soundRadius);

                Vector3 soundPointPosition = transform.position + new Vector3(randomX, randomY, randomZ);
                Transform soundPoint = Instantiate(soundPointPrefab, soundPointPosition, Quaternion.identity);
                soundPoint.SetParent(transform);

                // Рассчитываем звуковой радиус и угасание
                float distance = Vector3.Distance(transform.position, soundPointPosition);
                float volume = Mathf.Clamp01(1 - distance / fadeDistance);

                // Устанавливаем громкость точки звука
                AudioSource audioSource = soundPoint.GetComponent<AudioSource>();
                audioSource.volume = volume;
            }
        }
    }
}