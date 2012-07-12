# -*- coding: utf-8 -*-
import sys
import re
import clr
from utilidades import *
from System import DateTime, Guid
from Vulcano.Engine import RepoGeneral
"""
https://ironpython.svn.codeplex.com/svn/IronPython_1_1/Src/IronPython/Hosting/AdapterDict.cs
https://ironpython.svn.codeplex.com/svn/IronPython_1_1/Src/IronPython/Runtime/Dict.cs
"""

def evaluar_script(code, val, _data):
	data = _data
	hoy = DateTime.Now
	return eval(code)

def buscar_equivalencia(valor, mapeo, variable):
	if not valor: return None
	posibles = []
	if mapeo and mapeo.Catalogo: # varios catalogos, en orden de precedencia
		for x in mapeo.Catalogo.split(','):
			if not x: continue
			posibles.append(x.strip())
		#cats = [ posibles.append(x.strip()) for x in mapeo.Catalogo.split(',')]
	posibles.append(variable.Codigo)
	lista = cadena(valor).split(",")
	for v in lista:
		for categoria in posibles:
			equivalencia = catalogo.Buscar(categoria, v)
			if equivalencia: return equivalencia
	return None

# equivalencia entre valores de cubierta y entrepisos
cat_cub_entre = { \
'metalica' : 'metalica', \
'hormigon': 'hormigon', \
'madera_zinc': 'madera', \
'caña_zinc': 'madera_caña', \
'madera_teja': 'madera', \
'madera':'madera', \
}
cat_entre_cub = {}
for (k,v) in cat_cub_entre.iteritems():
	if v not in cat_entre_cub:
		cat_entre_cub[v] = k

def reglas_especiales(fuente, data):
	if data['num_pisos'] == 0:
		data['num_pisos'] = 1
	if data['anio_construccion'] > DateTime.Now.Year:
		data['anio_construccion'] = None
	if data['material_paredes'] in ('adobe','tapia','piedra') and not data['sist_estructural']:
		data['sist_estructural'] = 'pared_portante'
	if not data['cubierta']:
		cat = catalogo.Buscar('cubierta', data['entrepisos'])
		data['cubierta'] = cat if cat else None
	if not data['entrepisos']:
		cub = data['cubierta']
		cat = catalogo.Buscar('entrepisos', cub)
		if not cat and cub in cat_cub_entre:
			cat = cat_cub_entre[cub]
		data['entrepisos'] = cat if cat else None
		
def match_valor(var, valor, data):
	e = cadena(valor)
	for pos in var.Valores:
		if var.EsScript:
			if evaluar_script(pos.Expresion, valor, data):
				return pos.Puntajes
		else:
			for p in pos.Expresion.split(','):
				if e == p: return pos.Puntajes
	return None

def correr():
	print "Recuperando resultados"
	limite = 0
	datos = fuenteDatos.Valores(ciudad, limite)
	
	print "Borrando resultados"
	RepoGeneral.BorrarResultados(ficha.Nombre, "construccion", ciudad);
	print " A procesar %s" % datos.Count
	total = 0
	totalVariables = ficha.Variables.Count
	totalTemas = ficha.Temas.Count
	
	for fuente in datos:
		if limite > 0 and limite == total: break
		# que
		#if fuente.IdItem != 64887: continue
		#if fuente.IdItem != 64934: continue
		#if fuente.IdItem != 65434: continue # este en penipe tenia problemas
		f = {}
		f.update(fuente)
		res = proc.CrearResultado(fuente)
		data = {}
		completos = 0
		for var in ficha.Variables.Values:
			cod = var.Codigo
			data[cod] = None
			valor = get_valor(f[cod], var.Tipo) if cod in f else None
			#if mapeos.GetMapeo(cod) algo auxiliar para que no moleste
			if mapeos.Campos.ContainsKey(cod):
				m = mapeos.Campos[cod]
				for c in [x.strip() for x in m.Campo.split(',')]:
					if not c in f: continue
					valor = get_valor(f[c], m.Tipo)
					if m.Catalogo: 
						valor = buscar_equivalencia(valor, m, var)
					if m.Expresion:	
						valor = get_valor(valor, m.Tipo)
						valor = evaluar_script(m.Expresion, valor, f)
					else:
						#if m.Catalogo and not var.ValorEnPosibles(valor): valor = None
						if not var.EsScript and not var.ValorEnPosibles(valor): valor = None
					if valor != None and valor != '': break
			data[cod] = valor
		reglas_especiales(f, data)
		completos = 0
		for (k,v) in data.iteritems():
			det = proc.CrearDetalle(res, "evaluacion", k, cadena(v))
			det.Valor_numerico = getNumero(v)
			res.AddDetalle(det)
			if v == None or v == '': 
				continue
			completos += 1
		#calidad
		completo = completos / totalVariables
		#if completo != 1: continue
		det = proc.CrearDetalle(res, "calidad", "num_completos", None)
		det.Valor_numerico = completo
		res.AddDetalle(det)
		fuenteDatos.CompletarDatos(res, fuente)
		res.Completo = completo
		#calculo
		totales = {}
		for (k,v) in data.iteritems():
			if v == None: continue		
			var = ficha.Variables[k]
			pond = match_valor(var, v, data)
			if not pond: continue
			i = 0
			for ame in ficha.Temas:
				num = pond[i]
				po = var.Ponderaciones[i]
				i+=1
				if num  == None or po == None: continue
				calc = num * po
				det = proc.CrearDetalle(res, ame, k, cadena(v))
				det.Valor_numerico = num
				det.Calculado = calc
				res.AddDetalle(det)
				if i not in totales: totales[i] = 0
				totales[i] += calc
		for (ind, tot) in totales.iteritems():
			setattr(res, "Indicador%s" % ind, tot)
		resultados.Add(res)
		total+=1
		if total % 1000 == 0:
			print "Calculados %s" % total
	print "Procesado"

correr()

