using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class FaceObjectPlacer : MonoBehaviour
{
    public GameObject[] hatPrefabs; // Array para los prefabs de sombreros
    public GameObject mustachePrefab; // Prefab del bigote

    private ARFaceManager faceManager; // Referencia al ARFaceManager
    private HashSet<ARFace> processedFaces = new HashSet<ARFace>(); // Para rastrear caras procesadas

    void Start()
    {
        // Inicializar ARFaceManager
        faceManager = GetComponent<ARFaceManager>();

        // Validar los prefabs
        if (hatPrefabs == null || hatPrefabs.Length == 0)
        {
            Debug.LogError("No hay sombreros configurados en hatPrefabs.");
        }

        if (mustachePrefab == null)
        {
            Debug.LogError("No hay bigote configurado en mustachePrefab.");
        }

        if (faceManager == null)
        {
            Debug.LogError("ARFaceManager no está asignado en el objeto.");
        }
    }

    void Update()
    {
        // Verificar si se han detectado rostros
        bool facesDetected = faceManager.trackables.count > 0;
        if (facesDetected)
        {
            Debug.Log("Caras detectadas: " + faceManager.trackables.count);
        }
        else
        {
            Debug.Log("No se detectaron caras.");
        }

        // Iterar sobre todas las caras detectadas
        foreach (var face in faceManager.trackables)
        {
            if (!processedFaces.Contains(face))
            {
                // Colocar el bigote y un sombrero solo una vez por cara
                PlaceMustache(face);
                PlaceRandomHat(face);

                // Marcar la cara como procesada
                processedFaces.Add(face);
            }
        }
    }

    private void PlaceMustache(ARFace face)
    {
        // Calcular posición del bigote
        Vector3 mustachePosition = face.transform.position + new Vector3(0, -0.05f, 0);
        GameObject mustache = Instantiate(mustachePrefab, mustachePosition, Quaternion.identity);
        mustache.transform.parent = face.transform; // Hacer que el bigote siga la cara
    }

    private void PlaceRandomHat(ARFace face)
    {
        // Seleccionar un sombrero aleatorio
        int randomIndex = Random.Range(0, hatPrefabs.Length);
        Vector3 hatPosition = face.transform.position + new Vector3(0, 0.15f, 0); // Ajusta según pruebas
        GameObject hat = Instantiate(hatPrefabs[randomIndex], hatPosition, Quaternion.identity);
        hat.transform.parent = face.transform; // Hacer que el sombrero siga la cara
    }

    void OnEnable()
    {
        // Subscribirse al evento de cambios en caras
        if (faceManager != null)
        {
            faceManager.facesChanged += OnFacesChanged;
        }
    }

    void OnDisable()
    {
        // Desubscribirse del evento al desactivar el script
        if (faceManager != null)
        {
            faceManager.facesChanged -= OnFacesChanged;
        }
    }

    private void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        // Manejar caras que desaparecen
        foreach (var removedFace in args.removed)
        {
            processedFaces.Remove(removedFace); // Eliminar de la lista de procesadas
        }

        // Puedes agregar lógica adicional aquí si quieres realizar alguna acción cuando las caras cambian
    }
}
