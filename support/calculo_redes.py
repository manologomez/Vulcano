# -*- coding: utf-8 -*-
import sys
import re
import clr
from utilidades import *
from System import DateTime, Guid
from Vulcano.Engine import *#RepoGeneral, AppNotifier
from Vulcano.App import *

def evaluar_script(code, val, _data):
	data = _data
	hoy = DateTime.Now
	return eval(code)
		
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

def especial_por(valor):
	numero = getNumero(valor)
	if numero == None: return None
	if numero < 1: numero = numero*100
	if numero <= 0: return 'sin servicio'
	if numero > 80: return 'alta'
	if numero > 50 and numero <= 80: return 'moderada'
	if numero < 50: return 'baja'
	return None

#sirve lo mismo que catalogos por lo pronto
def transform(valor, var):
	txt = valor.strip().lower()
	if txt == 'hi': txt = 'ac'
	if var.Codigo == 'intervencion':
		if txt == 'personal y equipos necesarios': txt = 'alta'
	if var.ValorEnPosibles(txt): return txt
	for p in var.Valores:
		if p.Descripcion.lower() == txt:
			return p.Expresion
	return None

def correr():
	print " A procesar %s" % fuentes.Count
	total = 0
	for fuente in fuentes:
		f = {}
		f.update(fuente)
		if not proc.MapaFichas.ContainsKey(fuente.Tipo):
			continue
		#print fuente.Tipo, f
		ficha = proc.MapaFichas[fuente.Tipo]
		res = proc.CrearResultado(fuente)
		data = {}
		for var in ficha.Variables.Values:
			cod = var.Codigo
			data[cod] = None
			valor = cadena(f[cod])
			if not valor or valor == '': continue
			if var.Codigo == "cobertura":
				valor = especial_por(f[cod])
			else:
				valor = transform(valor, var)
			data[cod] = valor
		completos = 0
		for (k,v) in data.iteritems():
			det = proc.CrearDetalle(res, "evaluacion", k, cadena(v))
			det.Valor_numerico = getNumero(v)
			res.AddDetalle(det)
			if v == None or v == '': 
				continue
			completos += 1
		#calidad
		completo = completos / ficha.Variables.Count
		det = proc.CrearDetalle(res, "calidad", "num_completos", None)
		det.Valor_numerico = completo
		res.AddDetalle(det)
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
		if total % 10 == 0:
			print "Calculados %s" % total
	print "Procesado"

correr()

