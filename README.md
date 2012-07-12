Vulcano
=======

'Physical Vulnerability and Risk assessment software for cities.'

Software para c�lculo de indicadores de vulnerabilidad y riesgos para ciudades de Ecuador
con base en la metodolog�a desarrollada en el proyecto SGNR/PNUD en el a�o 2011.

Requerimientos del proyecto de c�digo
-------------------------------------

* Visual Studio 2010 SP1
* NuGet (�ltima versi�n)
* SQL Server 2005 o superior

Los paquetes adicionales se descargar�n autom�ticamente al primer build de acuerdo a la configuraci�n
de NuGet.

Archivos Adicionales
--------------------

La carpeta /support contiene los archivos adicionales del proyecto de acuerdo al piloto de la metololog�a incluyendo
datos de varias ciudades.

* FICHAS_INDICADORES.xlsx : Formato y valores de variables por tema para c�lculo de indicadores
* CATALOGOS_SISTEMA.xlsx : Valores de las variables de la fuente de datos original y equivalencias con variables de la metodolog�a
* Mapeos_fuentes.xlsx : Mapeo variable <-> campo de la base para extracci�n de datos
* Plantilla_histograma.xlsx : Requerido para el m�dulo de reportes del sistema.

Los dem�s archivos corresponden a los scripts en IronPython para la ejecuci�n de los procesos de c�lculo.