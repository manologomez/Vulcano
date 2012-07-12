# -*- coding: utf-8 -*-
import sys
import re
import clr
import System
from System import Array, DateTime, Environment
from System.IO import Path, File, Directory, SearchOption
clr.AddReference('System.Data')
clr.AddReference('SqlDataClasses.dll')
from System.Data import *
from System.Text import RegularExpressions as regex

def make_key(*args):
	"""Construye un string uniendo varios objetos, bueno para claves en hash"""
	strings = ["%s" % s for s in args]
	return "_".join(strings)

def flatten(obj):
	""" HORRIBLE forma recursiva de transformar una serie de objetos en representacion basica para serializar 
	Para esto seria mejor usar el modulo types y tal vez inspect
	"""
	if obj is None: return None
	
	if isinstance(obj, dict):
		res = {}
		for k,v in obj.items():
			res[k] = flatten(v)
		return res
	props = dir(obj)
	# segun esto, esto contienen list, tuple, set, frozetset
	if '__iter__' in props and '__getitem__' in props:
		res = []
		for i in obj:
			res.append(flatten(i))
		return res
	# objetos normalitos, clases
	if hasattr(obj, '__dict__'):
		vals = vars(obj)
		res = {}
		for k,v in vals.items():
			res[k] = flatten(v)
		return res
	return obj

def get_valor(valor, tipo):
	"""Transforma un valor al tipo definido. Tipos posibles:
	int, float, cadena, varchar, numero_cadena
	numero_cadena se asegura de devolver un número como cadena (por excel más que nada)
	"""
	if valor is None: return None
	if tipo == 'int':
		return getEntero(valor)
	if tipo == 'float':
		return getNumero(valor)
	if tipo == 'codigo':
		return getCodigo(valor)
	if tipo == 'varchar' or tipo == 'cadena':
		return "%s" % valor
	if tipo == 'numero_cadena':
		return get_numero_cadena(valor)
	return valor

def eval_code(_code, _val, _ctx=None):
	"""Evaluación dinámica de código. Se debería generalizar más"""
	val = _val
	hoy = DateTime.Now #variable auxiliar
	return eval(_code)

def toDict(obj):
	""" Convierte un diccionario de python en un diccionario genérico de .net (string, obj) """
	d = System.Collections.Generic.Dictionary[System.String, System.Object]()
	for k in obj.keys():
		d[k] = 	obj[k]
	return d

def readerDict(reader):
	""" DataReader a lista de diccionarios python tomando en cuenta DBNull """
	lista = []
	while reader.Read():
		c = reader.FieldCount
		obj = {}
		for i in range(c):
			n = reader.GetName(i)
			val = reader[i]
			if isinstance(val, System.DBNull): val = None
			obj[n] = val
		lista.append(obj)
	return lista

def readerCurrentDict(reader):
	""" Extrae el contenido actual de un DataReader a un diccionario de python """
	c = reader.FieldCount
	obj = {}
	for i in range(c):
		n = reader.GetName(i)
		val = reader[i]
		if isinstance(val, System.DBNull): val = None
		obj[n] = val
	return obj

# Funciones genericas de conversion

def cadena(obj):
	"""Representación de string de un objeto. Usa el truco para cadenas especiales"""
	if obj == None: return obj
	return "%s" % obj

reg_numero = re.compile("^\-?[0-9]+([\.,][0-9]+)?$", re.UNICODE)

def getNumero(val):
	"""Comprueba si el valor es numérico o es un numero como cadena y devuelve un float"""
	if val == None: return None
	if isinstance(val, int) or isinstance(val, float): return val
	t = ("%s" % val).strip()
	m = reg_numero.match(t)
	if m:
		txt = t.replace(',','.')
		return float(txt)
	return None

def getEntero(val):
	"""Devuelve un entero a partir del valor si se puede representar como tal"""
	num = getNumero(val)
	return int(num) if num != None else None

def getCodigo(val):
	"""Trata de devolver el texto de un código, incluyendo si hay varios"""
	if val == None: return None
	txt = ("%s" % val).replace(";",',')
	if txt.find(',') > 0:
		return txt
		
	# cambiar esto para que coja codigos multiples 02;03;04, etc
	v = getEntero(val)
	if v != None: return str(v)
	return None

def get_numero_cadena(val):
	"""Devuelve un texto comprobando si el valor es un numero entero de excel (.. .0)"""
	if val == None: return None
	txt = "%s" % val
	return txt if not txt.endswith('.0') else txt[:-2]
		
def getLastId(helper):
	"""Devuelve el ultimo valor de identidad de SQL Server. No muy confiable"""
	sql = "SELECT @@IDENTITY as id"
	#sql = "SELECT scope_identity() as id"
	r = helper.ExecuteReader(sql)
	r.Read()
	id = r[0]
	r.Close()
	return id

# Clases para el cargador

class Registro:
	"""Representa un registro de predio con bloques (componentes) si existen"""
	def __init__(self):
		self.predio = {}
		self.bloques = []
		self.fila = 0

	def show(self):
		print "FILA %s PREDIO:" % self.fila
		print self.predio
		print "BLOQUES: %s" % len(self.bloques)
		for b in self.bloques:
			print "---"
			print b

def guardar(helper, item, tx=None):
	"""Guarda un predio en la db"""
	helper.ExecuteInsert('predio', item.predio)
	lastid = getLastId(helper)

	for b in item.bloques:
		num = b['num_construccion']
		for (k,v) in b.items():
			if k == 'num_construccion': continue
			data = dict( id_predio=lastid, num_construccion=num )
			data['clave'] = k
			data['valor_texto'] = ("%s" % v).strip()
			if isinstance(v, int) or isinstance(v, float):
				data['valor_numero'] = v
			helper.ExecuteInsert('dato_predio', data)
			data.clear()
	pass

def guardarLista(conn, helper, lista, callback=None): # EXT-> Esto se debe mejorar, meter en una clase p.ej
	"""Guarda una lista de predios procesados dentro de una transacción"""
	tx = conn.BeginTransaction()
	helper.SetDefaultTransaction(tx)
	i = 0
	try:
		for item in lista:
			guardar(helper, item, tx)
			if callback and callable(callback):
				callback(i)
			i += 1
		tx.Commit()
	except Exception as ex:
		tx.Rollback()
		print "error"
		raise ex

	return len(lista)


