import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Libro {
  id?: number;
  idExterno: string;
  titulo: string;
  autores: string;
  anioPublicacion?: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="contenedor">
      <header>
        <h1>LumyBook</h1>
        <p>Tu buscador personal de libros</p>
      </header>

      <nav>
        <button (click)="cambiarVista('buscar')" [class.activo]="vista() === 'buscar'">🔍 Buscar</button>
        <button (click)="cargarFavoritos()" [class.activo]="vista() === 'favoritos'">❤️ Mis Favoritos</button>
      </nav>

      <section *ngIf="vista() === 'buscar'">
        <div class="buscador">
          <input [(ngModel)]="textoBusqueda" (keyup.enter)="buscarLibros()" placeholder="Escribe un título (ej. Principito)...">
          <button (click)="buscarLibros()">Ir</button>
        </div>

        <div class="grilla">
          <div *ngFor="let libro of resultados()" class="tarjeta">
            <h3>{{ libro.titulo }}</h3>
            <p>{{ libro.autores }} ({{ libro.anioPublicacion }})</p>
            <button class="btn-guardar" (click)="guardar(libro)">Guardar en la biblioteca</button>
          </div>
        </div>
      </section>

      <section *ngIf="vista() === 'favoritos'">
        <div class="grilla">
          <div *ngFor="let fav of favoritos()" class="tarjeta fav">
            <h3>{{ fav.titulo }}</h3>
            <p>{{ fav.autores }}</p>
            <button class="btn-eliminar" (click)="eliminar(fav.id!)">Eliminar</button>
          </div>
          <p *ngIf="favoritos().length === 0">Aún no tienes libros guardados.</p>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .contenedor { max-width: 800px; 
                  margin: 0 auto; 
                  font-family: 'Playfair Display', serif;
                  padding: 20px; 
                }

    header { text-align: center; 
             margin-bottom: 30px; 
            }
    
    h1 { color: #1D2066; 
         margin: 0; 
        }
    
    nav { display: flex; 
          justify-content: center; 
          gap: 10px; 
          margin-bottom: 20px; 
        }
    
    button { padding: 10px 20px; 
             cursor: pointer; 
             border: none; 
             border-radius: 5px; 
             background: white;
             font-family: Inter, serif;
            }
    
    button.activo { background: #1D2066; 
                    color: white; 
                  }
    
    .buscador { display: flex; 
                gap: 10px; 
                margin-bottom: 20px;
                font-family: Inter, serif;
              }

    input { flex: 1; 
            padding: 10px; 
            border-radius: 5px; 
            border: 1px solid #ccc; 
          }
    
    .grilla { display: grid; 
              grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); 
              gap: 15px; 
            }
    
    .tarjeta { border: 1px solid #ddd; 
               padding: 15px; 
               border-radius: 8px; 
               background: white; 
               box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
    
    .fav { border-left: 
           5px solid #121440; 
          }
    
    .btn-guardar { background: #121440; 
                   color: white; 
                   width: 100%; 
                   margin-top: 10px; 
                  }
    
    .btn-eliminar { background: #8F0101; 
                    color: white; 
                    width: 100%; 
                    margin-top: 10px; 
                  }
  `]
})
export class App {
  http = inject(HttpClient);
  apiUrl = 'http://localhost:5058/api';
  vista = signal('buscar');
  textoBusqueda = '';
  resultados = signal<Libro[]>([]);
  favoritos = signal<Libro[]>([]);

  cambiarVista(v: string) { this.vista.set(v); }

  buscarLibros() {
    if(!this.textoBusqueda) return;
    this.http.get<Libro[]>(`${this.apiUrl}/buscar?q=${this.textoBusqueda}`)
      .subscribe(res => this.resultados.set(res));
  }

  cargarFavoritos() {
    this.cambiarVista('favoritos');
    this.http.get<Libro[]>(`${this.apiUrl}/favoritos`)
      .subscribe(res => this.favoritos.set(res));
  }

  guardar(libro: Libro) {
    this.http.post(`${this.apiUrl}/favoritos`, libro).subscribe({
      next: () => alert('¡Guardado en Lumy!'),
      error: (e) => {
        if(e.status === 409) alert('¡Ya tienes este libro en Lumy!');
        else alert('Error al guardar');
      }
    });
  }

  eliminar(id: number) {
    if(confirm('¿Seguro que quieres borrarlo?')) {
      this.http.delete(`${this.apiUrl}/favoritos/${id}`)
        .subscribe(() => this.cargarFavoritos());
    }
  }
}