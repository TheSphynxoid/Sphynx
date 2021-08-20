#include "pch.h"
#include "GLRenderer.h"
#define GLFW_INCLUDE_NONE
//Before IMGUI.
#include <GLFW/glfw3.h>
//IMGUI gets loaded here.
#include "GLWindow.h"
#include "glad/glad.h"
//#define GLFW_EXPOSE_NATIVE_WIN32
//#include "GLFW/glfw3native.h"
#include "GLMaterial.h"
#include "GLMesh.h"
#include "GLShader.h"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#ifdef Platform_Windows
extern "C" {
	__declspec(dllexport) DWORD NvOptimusEnablement = 1;
}
#endif
void Sphynx::Core::Graphics::GL::GLRenderer::RendererResizeEvent(Events::OnWindowResize& e)
{
	glViewport(0, 0, e.Width, e.Height);
}

void Sphynx::Core::Graphics::GL::GLRenderer::Start(IWindow* app)
{
	app->GetEventSystem()->Subscribe(this, &GLRenderer::RendererResizeEvent);
	GLMaterial::DefaultMaterial = GLMaterial::CreateDefaultMaterial();
	DefaultRenderObject = RenderObject(nullptr, &GLMaterial::DefaultMaterial, { 0,0,0 }, { 0,0,0,0 });
	//GL features.
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glEnable(GL_DEPTH_TEST);
}

void Sphynx::Core::Graphics::GL::GLRenderer::Render()
{
	//packed.
	for (auto& pair : RenderQueue) {
		pair.second->front().mat->Bind();
		glUseProgram(pair.first);
		for (auto rend : *pair.second) {
			rend.mesh->Bind();
			GLMesh* mesh = (GLMesh*)rend.mesh;
			if (mesh->HasIndexArray()) {
				glDrawElements(GL_TRIANGLES, mesh->GetIndexBufferSize(), GL_INT, 0);
			}
			else {
				glDrawArrays(GL_TRIANGLES, 0, mesh->GetVertexBuffer()[0]->GetVertexBufferSize());
			}
		}
		//Clearing Queue for new loop.
		pair.second->clear();
	}
	//RenderQueue.clear();
}

void Sphynx::Core::Graphics::GL::GLRenderer::Clear()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
}

void Sphynx::Core::Graphics::GL::GLRenderer::OnSubmit(RenderObject rend)
{
	if (rend.mat == nullptr) {
		rend.mat = DefaultRenderObject.mat;
	}
	//TODO: Handle Empty Mesh, show some kind of error like a red box with text on top.
	if (rend.mesh == nullptr) {
	}
	//I think this is sorted.
	auto l = RenderQueue[((GLMaterial*)rend.mat)->ProgramId];
	if (l == nullptr) {
		l = new RenderObjectList();
		RenderQueue[((GLMaterial*)rend.mat)->ProgramId] = l;
	}
	l->push_back(rend);
}

void Sphynx::Core::Graphics::GL::GLRenderer::SetClearColor(Vec4 color)
{
	glClearColor(color.r, color.g, color.b, color.a);
}
