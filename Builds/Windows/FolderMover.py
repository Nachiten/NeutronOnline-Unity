import shutil
import os

def enterAndExit(exitCode=0):
    print("Presione ENTER para salir")
    input()
    exit(exitCode)

carpetaOrigen1 = ""
carpetaOrigen2 = ""

while True:
    carpetaOrigen1 = input("[FolderMover] Ingrese el nombre de la carpeta MonoBleedingEdge [ENTER para MonoBleedingEdge]:\n")
    if carpetaOrigen1 == "":
        carpetaOrigen1 = "MonoBleedingEdge"

    if os.path.exists(carpetaOrigen1):
        break

    print("[FolderMover] La carpeta ingresada [" + carpetaOrigen1 + "] no existe")

while True:
    carpetaOrigen2 = input("[FolderMover] Ingrese el nombre del ejecutable (sin el .exe):\n")
    carpetaOrigen2 = carpetaOrigen2 + "_Data"

    if os.path.exists(carpetaOrigen2):
        break

    print("[FolderMover] La carpeta ingresada [" + carpetaOrigen2 + "] no existe")

# Creo nombres de copias
carpetaOrigen1Copia = carpetaOrigen1 + " Copy"
carpetaOrigen2Copia = carpetaOrigen2 + " Copy"

try:
    # Creo directorios copia
    os.mkdir(carpetaOrigen1Copia)
    os.mkdir(carpetaOrigen2Copia)

    # Muevo las carpetas originales adentro de la copia
    shutil.move(carpetaOrigen1, carpetaOrigen1Copia)
    shutil.move(carpetaOrigen2, carpetaOrigen2Copia)

    # Renombro las copias para volver a los nombres originales
    os.rename(carpetaOrigen1Copia, carpetaOrigen1)
    os.rename(carpetaOrigen2Copia, carpetaOrigen2)

    print("[FolderMover] Las carpetas fueron creadas y movidas correctamente")

except Exception as e:
    print("[FolderMover] Error al mover las carpetas: " + str(e))
