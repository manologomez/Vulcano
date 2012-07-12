Vulcano
=======

'Physical Vulnerability and Risk assessment software for cities.'

Software para cálculo de indicadores de vulnerabilidad y riesgos para ciudades de Ecuador
con base en la metodología desarrollada en el proyecto SGNR/PNUD en el año 2011.

Requerimientos del proyecto de código
-------------------------------------

* Visual Studio 2010 SP1
* NuGet (última versión)
* SQL Server 2005 o superior

Los paquetes adicionales se descargarán automáticamente al primer build de acuerdo a la configuración
de NuGet.

Archivos Adicionales
--------------------

La carpeta /support contiene los archivos adicionales del proyecto de acuerdo al piloto de la metolología incluyendo
datos de varias ciudades.

* FICHAS_INDICADORES.xlsx : Formato y valores de variables por tema para cálculo de indicadores
* CATALOGOS_SISTEMA.xlsx : Valores de las variables de la fuente de datos original y equivalencias con variables de la metodología
* Mapeos_fuentes.xlsx : Mapeo variable <-> campo de la base para extracción de datos
* Plantilla_histograma.xlsx : Requerido para el módulo de reportes del sistema.

Los demás archivos corresponden a los scripts en IronPython para la ejecución de los procesos de cálculo.