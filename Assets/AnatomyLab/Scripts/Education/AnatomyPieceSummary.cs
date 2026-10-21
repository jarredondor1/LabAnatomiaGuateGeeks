using System;
using GuateGeeks.AnatomyLab.Core;

namespace GuateGeeks.AnatomyLab.Education {
// Las piezas identificadas tienen una explicación propia; las demás reciben contexto de su tipo o sistema.
public static class AnatomyPieceSummary {
 public static string Title(AnatomyStructure piece,string name,string system){
  if(!string.IsNullOrWhiteSpace(name)&&!name.Contains(piece.id))return name;
  var kind=Kind(piece.canonicalName);
  if(kind==null)kind=piece.systemId=="skeleton"?"Pieza ósea":piece.systemId=="respiratory"?"Pieza respiratoria":piece.systemId=="digestive"?"Pieza digestiva":piece.systemId=="reproductive"?"Pieza reproductora":piece.systemId=="urinary"?"Pieza urinaria":piece.systemId=="nervous"?"Tejido nervioso":piece.systemId=="integumentary"?"Tejido de la piel":"Pieza anatómica";
  var side=(piece.canonicalName??"").ToLowerInvariant().StartsWith("left ")?" · lado izquierdo":(piece.canonicalName??"").ToLowerInvariant().StartsWith("right ")?" · lado derecho":"";
  return kind+" · "+Region((piece.canonicalName??"").ToLowerInvariant(),piece.originalPosition.y)+side;
 }

 public static string Describe(AnatomyStructure piece,string system){
  var name=piece.canonicalName==null?"":piece.canonicalName.Trim().ToLowerInvariant();
  if(name.StartsWith("right ",StringComparison.Ordinal)||name.StartsWith("left ",StringComparison.Ordinal))name=name.Substring(name.IndexOf(' ')+1);
  switch(name){
   case "stomach": return Summary("órgano muscular situado entre el esófago y el intestino delgado. Sus paredes se contraen para mover el alimento y su revestimiento produce jugos gástricos.","mezcla los alimentos con jugos gástricos e inicia la digestión de las proteínas. El contenido se transforma en quimo y pasa de forma gradual al intestino delgado.");
   case "femur": return Summary("hueso largo del muslo que une la cadera con la rodilla. Es el principal soporte óseo de la pierna.","soporta el peso del cuerpo y transmite fuerzas entre la cadera y la rodilla. Junto con músculos y articulaciones, permite caminar, levantarse y mantener el equilibrio.");
   case "kidney": return Summary("órgano del sistema urinario situado a cada lado de la columna, detrás de la cavidad abdominal.","filtra la sangre para retirar desechos y exceso de agua, formando la orina. También ayuda a regular el equilibrio de líquidos y sales del cuerpo.");
   case "kidney capsule": return Summary("capa de tejido fibroso que recubre directamente el riñón y lo separa de los tejidos vecinos.","protege al riñón y ayuda a conservar su forma. Forma una envoltura resistente alrededor del tejido donde se produce la orina; la filtración ocurre en el interior del órgano.");
   case "ureter": return Summary("conducto muscular que conecta la pelvis de cada riñón con la vejiga urinaria.","transporta la orina del riñón a la vejiga mediante contracciones sucesivas de su pared. Su función es conducirla, no almacenarla ni producirla.");
   case "lung": return Summary("órgano respiratorio del tórax que contiene bronquios, vasos sanguíneos y alvéolos.","en los alvéolos permite que el oxígeno pase a la sangre y que el dióxido de carbono salga de ella. La ventilación renueva el aire necesario para ese intercambio.");
   case "liver": return Summary("órgano grande del abdomen, situado bajo el diafragma, que recibe sangre del aparato digestivo.","procesa y almacena nutrientes, transforma distintas sustancias y produce bilis. La bilis facilita la digestión de las grasas en el intestino.");
   case "brain": return Summary("parte principal del sistema nervioso central, protegida por el cráneo y conectada con la médula espinal.","recibe y procesa información sensorial, coordina movimientos y participa en funciones como memoria, lenguaje y regulación de otros órganos.");
   case "spleen": return Summary("órgano del sistema linfático situado en la parte superior izquierda del abdomen, cerca del estómago.","filtra la sangre y participa en la respuesta inmunitaria. Ayuda a retirar células sanguíneas envejecidas y a reconocer agentes extraños.");
   case "pancreas": return Summary("órgano glandular situado detrás del estómago, conectado con el intestino delgado.","produce enzimas que ayudan a digerir alimentos y hormonas, como la insulina, que regulan la glucosa en sangre.");
   case "heart": return Summary("órgano muscular del tórax dividido en cavidades y conectado con los grandes vasos sanguíneos.","bombea sangre hacia los pulmones para su oxigenación y hacia el resto del cuerpo para distribuir oxígeno y nutrientes.");
   case "ovary": return Summary("órgano reproductor femenino situado a un lado del útero y próximo a la trompa uterina.","desarrolla y libera óvulos durante el ciclo ovárico. También produce hormonas, principalmente estrógenos y progesterona, que intervienen en el ciclo reproductivo.");
   case "uterus": case "body of uterus": return Summary("órgano muscular hueco de la pelvis, situado entre la vejiga y el recto. Su revestimiento interno cambia a lo largo del ciclo menstrual.","recibe al embrión y permite su desarrollo durante el embarazo. Si no ocurre un embarazo, parte del revestimiento se desprende durante la menstruación.");
   case "urinary bladder": return Summary("órgano hueco y muscular de la pelvis que recibe la orina procedente de los uréteres.","almacena la orina de forma temporal. Cuando se vacía, su pared muscular se contrae y la orina sale por la uretra.");
   case "trachea": return Summary("tubo respiratorio que desciende desde la laringe y se divide en los bronquios principales.","conduce el aire hacia los pulmones. Sus anillos de cartílago ayudan a mantener abierta la vía aérea durante la respiración.");
   case "esophagus": return Summary("tubo muscular que une la faringe con el estómago y pasa por el tórax.","impulsa alimentos y líquidos hacia el estómago mediante movimientos coordinados de su pared llamados peristalsis.");
   case "aorta": return Summary("arteria de mayor calibre que sale del ventrículo izquierdo del corazón y se ramifica por el cuerpo.","distribuye sangre oxigenada hacia los órganos y tejidos a través de sus ramas arteriales.");
   case "vagina": return Summary("conducto muscular y elástico que comunica el cuello del útero con el exterior del cuerpo.","permite la salida del flujo de la menstruación y constituye el canal por el que nace el bebé durante el parto. También comunica los genitales externos con el cuello uterino.");
   case "uterine cervix": return Summary("parte inferior y estrecha del útero que se proyecta hacia la vagina. En su interior se encuentra el canal cervical.","comunica el útero con la vagina y produce moco que cambia a lo largo del ciclo menstrual. Durante el parto se dilata para permitir el paso del bebé.");
   case "cerebral white matter": return Summary("tejido interno del cerebro formado principalmente por haces de prolongaciones nerviosas.","conecta distintas regiones del sistema nervioso y permite que las señales viajen entre ellas. Así integra información sensorial, motora y cognitiva.");
   case "testis": return Summary("gónada masculina alojada en el escroto, formada por túbulos donde se desarrollan las células reproductoras.","produce espermatozoides y secreta testosterona, hormona que participa en la función reproductiva y en otros procesos corporales.");
   case "prostate": return Summary("glándula del aparato reproductor masculino situada bajo la vejiga y alrededor de una parte de la uretra.","aporta líquido al semen. Sus secreciones contribuyen al medio en el que se transportan los espermatozoides.");
   case "ductus deferens": case "vas deferens": return Summary("conducto muscular que va desde el epidídimo hacia los conductos eyaculadores.","transporta espermatozoides durante la eyaculación mediante contracciones de su pared; no es el lugar donde se producen.");
   case "penis": return Summary("órgano genital externo masculino que contiene tejido eréctil y la parte final de la uretra.","permite la salida de la orina y, durante la eyaculación, del semen. El tejido eréctil cambia de volumen al llenarse de sangre.");
   case "skin": case "skin of body": return Summary("cubierta externa del cuerpo, compuesta por capas de tejido que separan el medio interno del exterior.","actúa como barrera protectora, participa en la sensibilidad y ayuda a regular la temperatura corporal.");
  }
  if(name.EndsWith("retina",StringComparison.Ordinal))return Summary("capa sensible a la luz que reviste la parte interna del ojo. Contiene células que detectan los estímulos luminosos.","convierte la luz en señales nerviosas. Estas señales se procesan en la retina y viajan por el nervio óptico hacia el cerebro, donde contribuyen a formar la visión.");
  if(name.Contains("ampulla of")&&name.Contains("uterine tube"))return Summary("segmento ensanchado de la trompa uterina, situado entre el extremo cercano al ovario y la porción que llega al útero.","ayuda a transportar el óvulo hacia el útero. Es la región donde con mayor frecuencia se encuentran óvulo y espermatozoide para la fecundación.");
  if(name.EndsWith(" bronchial tree",StringComparison.Ordinal))return Summary("red de bronquios que se ramifica dentro de una región del pulmón hasta llegar a vías de menor tamaño.","conduce y distribuye el aire hacia las zonas donde ocurre el intercambio de gases.");
  var kind=Kind(name);
  var region=Region(name,piece.originalPosition.y);
  var side=piece.canonicalName!=null&&piece.canonicalName.ToLowerInvariant().Contains("left ")?" izquierdo":piece.canonicalName!=null&&piece.canonicalName.ToLowerInvariant().Contains("right ")?" derecho":"";
  if(kind=="Arteria")return Summary("vaso sanguíneo arterial"+side+" de "+region+". Su pared soporta la presión de la sangre impulsada por el corazón.","conduce sangre hacia los tejidos de "+region+" por medio de sus ramas. Esta pieza permite seguir una parte concreta del trayecto arterial.");
  if(kind=="Vena")return Summary("vaso sanguíneo venoso"+side+" de "+region+", relacionado con la red de vasos de los tejidos cercanos.","recoge sangre de esa región y la conduce de regreso al corazón. Al observar su recorrido se puede comparar con las arterias vecinas.");
  if(kind=="Nervio")return Summary("haz de fibras nerviosas"+side+" de "+region+" que conecta tejidos con otras partes del sistema nervioso.","transmite señales sensitivas, motoras o autónomas según las fibras que contiene. Su recorrido ayuda a estudiar la comunicación con la región que alcanza.");
  if(kind=="Músculo")return Summary("músculo"+side+" de "+region+", formado por fibras capaces de acortarse al recibir señales nerviosas.","genera fuerza al contraerse y contribuye al movimiento o a la estabilidad de las estructuras con las que se relaciona.");
  if(kind=="Diente")return Summary("pieza dura de la boca fijada al maxilar o a la mandíbula por sus raíces.","participa en la masticación al cortar o triturar los alimentos, facilitando la digestión posterior.");
  if(kind=="Ligamento")return Summary("banda resistente de tejido que conecta huesos cerca de una articulación.","estabiliza la articulación y ayuda a limitar movimientos que podrían exceder su rango habitual.");
  if(kind=="Tendón")return Summary("banda de tejido resistente que une un músculo con un hueso.","transmite al hueso la fuerza generada cuando el músculo se contrae, haciendo posible el movimiento.");
  if(kind=="Ligamento")return Summary("banda resistente de tejido de "+region+" que une estructuras cercanas a una articulación.","estabiliza la articulación y limita movimientos excesivos durante la actividad normal.");
  if(kind=="Tendón")return Summary("cordón de tejido resistente de "+region+" que conecta un músculo con el esqueleto.","transmite al hueso la fuerza producida por el músculo y ayuda a ejecutar el movimiento.");
  string identity="«"+Title(piece,piece.displayName,system)+"»: "+Identity(piece,region);
  return Summary(identity,"en esta región, "+GroupFunction(piece.systemId)+" Su forma y posición permiten estudiar su relación con las piezas cercanas.");
 }

 static string Identity(AnatomyStructure piece,string region){
  var name=(piece.canonicalName??"").ToLowerInvariant();
  if(name.Contains("bone")||name.Contains("vertebra")||piece.systemId=="skeleton")return "elemento óseo de "+region+" que integra el armazón del cuerpo. Su forma y posición muestran con qué otras piezas se articula o a cuáles protege.";
  if(name.Contains("bronch")||name.Contains("trache")||piece.systemId=="respiratory")return "segmento del aparato respiratorio de "+region+". Su posición muestra la continuidad de la vía aérea o del tejido donde ocurre el intercambio gaseoso.";
  if(name.Contains("intestin")||name.Contains("colon")||piece.systemId=="digestive")return "parte del aparato digestivo de "+region+" que se relaciona con el tránsito, la transformación o la absorción del alimento.";
  if(piece.systemId=="reproductive")return "parte del aparato reproductor de "+region+". Su ubicación permite identificar las vías y órganos que participan en la producción, transporte o alojamiento de células reproductoras.";
  if(piece.systemId=="urinary")return "parte del aparato urinario de "+region+". Se integra en el recorrido de la orina desde su formación hasta su eliminación.";
  if(piece.systemId=="nervous")return "tejido del sistema nervioso de "+region+" conectado con otras regiones por vías de información.";
  if(piece.systemId=="lymphatic")return "elemento de la red linfática de "+region+" relacionado con el drenaje de líquidos y la defensa del organismo.";
  if(piece.systemId=="integumentary")return "tejido de la cubierta corporal de "+region+" que separa o conecta la superficie con estructuras profundas.";
  if(piece.systemId=="endocrine")return "tejido glandular de "+region+" que participa en la regulación hormonal del organismo.";
  if(piece.systemId=="arterial")return "rama de la red arterial de "+region+" que participa en la distribución de sangre hacia los tejidos cercanos.";
  if(piece.systemId=="venous")return "rama de la red venosa de "+region+" que recoge sangre de los tejidos cercanos.";
  if(piece.systemId=="muscular")return "componente muscular de "+region+" que interviene en la producción o transmisión de fuerza.";
  if(piece.systemId=="cardiovascular")return "componente de la circulación situado en "+region+" y relacionado con el paso de sangre por el cuerpo.";
  if(piece.systemId=="pregnancy")return "estructura representada para estudiar el embarazo y sus relaciones anatómicas en "+region+".";
  return "tejido anatómico de "+region+" relacionado con las piezas vecinas. Su forma y ubicación ayudan a reconocer cómo se organiza esta zona del cuerpo.";
 }

 static string Region(string name,float y){
  if(name.Contains("cerebr")||name.Contains("brain")||name.Contains("cranial")||name.Contains("skull")||name.Contains("eye")||name.Contains("optic")||name.Contains("nasal"))return "la cabeza";
  if(name.Contains("cervic")||name.Contains("neck")||name.Contains("laryn")||name.Contains("pharyn"))return "el cuello";
  if(name.Contains("thorac")||name.Contains("chest")||name.Contains("pulmon")||name.Contains("lung")||name.Contains("heart")||name.Contains("bronch"))return "el tórax";
  if(name.Contains("abdomin")||name.Contains("gastr")||name.Contains("hepatic")||name.Contains("intestin")||name.Contains("colon")||name.Contains("kidney")||name.Contains("renal"))return "el abdomen";
  if(name.Contains("pelv")||name.Contains("uter")||name.Contains("ovar")||name.Contains("prostat")||name.Contains("testi")||name.Contains("bladder"))return "la pelvis";
  if(name.Contains("brach")||name.Contains("humer")||name.Contains("radial")||name.Contains("ulnar")||name.Contains("hand")||name.Contains("finger")||name.Contains("carpal"))return "el miembro superior";
  if(name.Contains("femor")||name.Contains("tibial")||name.Contains("fibular")||name.Contains("foot")||name.Contains("plantar")||name.Contains("leg"))return "el miembro inferior";
  if(y>1.42f)return "la cabeza";if(y>1.05f)return "el tronco superior";if(y>.62f)return "el tronco y la pelvis";return "el miembro inferior";
 }

 static string Summary(string identity,string function){
  return "Qué es: "+identity+"\n\nFunción principal: "+function;
 }

 static string Kind(string canonical){
  if(string.IsNullOrWhiteSpace(canonical))return null;
  var name=canonical.Trim().ToLowerInvariant();
  if(name.Contains("artery")||name.Contains("arteries"))return "Arteria";
  if(name.Contains(" vein")||name.StartsWith("vein")||name.Contains(" veins"))return "Vena";
  if(name.Contains(" nerve")||name.StartsWith("nerve")||name.Contains(" nerves"))return "Nervio";
  if(name.Contains(" muscle")||name.StartsWith("muscle")||name.Contains(" muscles"))return "Músculo";
  if(name.EndsWith(" tooth",StringComparison.Ordinal)||name=="tooth")return "Diente";
  if(name.EndsWith(" ligament",StringComparison.Ordinal)||name=="ligament")return "Ligamento";
  if(name.EndsWith(" tendon",StringComparison.Ordinal)||name=="tendon")return "Tendón";
  return null;
 }

 static string GroupFunction(string id){
  switch(id){
   case "skeleton": return "brinda sostén y protección y participa en el movimiento.";
   case "muscular": return "produce movimiento mediante contracciones.";
   case "arterial": return "distribuye sangre desde el corazón hacia los tejidos.";
   case "venous": return "devuelve sangre hacia el corazón.";
   case "nervous": return "recibe señales y coordina respuestas.";
   case "respiratory": return "permite el intercambio de oxígeno y dióxido de carbono.";
   case "digestive": return "procesa alimentos y absorbe nutrientes.";
   case "urinary": return "forma, transporta y elimina orina.";
   case "reproductive": return "interviene en la reproducción.";
   case "endocrine": return "produce hormonas que regulan funciones corporales.";
   case "lymphatic": return "transporta linfa y participa en la defensa del organismo.";
   case "cardiovascular": return "ayuda a mantener la circulación sanguínea.";
   case "connective": return "sostiene y une otras estructuras.";
   case "integumentary": return "protege la superficie corporal.";
   case "pregnancy": return "se relaciona con el embarazo y el desarrollo fetal.";
   default: return "organiza y conecta los tejidos de esta región corporal.";
  }
 }
}
}
