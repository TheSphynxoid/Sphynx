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
	//To Use an Nvidia GPU if available.
	__declspec(dllexport) DWORD NvOptimusEnablement = 1;
}
#endif

void Sphynx::Core::Graphics::GL::GLRenderer::RendererResizeEvent(Events::OnWindowResize& e)
{
	glViewport(0, 0, e.Width, e.Height);
}

void Sphynx::Core::Graphics::GL::GLRenderer::Start(IWindow* app)
{
	GLMaterial::DefaultMaterial = GLMaterial::CreateDefaultMaterial();
	DefaultRenderObject = RenderObject(nullptr, &GLMaterial::DefaultMaterial);
	//GL features.
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glEnable(GL_DEPTH_TEST);
	//glEnable(GL_SCISSOR_TEST);
}

void Sphynx::Core::Graphics::GL::GLRenderer::Render()
{
	//TODO: Batch Rendering.
	for (auto& pair : RenderQueue) {
		pair.second->front().mat->Bind();
		for (auto rend : *pair.second) {
			rend.mesh->Bind();
			for (auto& uni : ((GLMaterial*)rend.mat)->uniforms) {
				((GLUniform*)uni)->BindLocation();
			}
			GLMesh* Mesh = (GLMesh*)rend.mesh;
			if (Mesh->HasIndexArray()) {
				glDrawElements((GLenum)Mesh->GetRenderMode(), Mesh->GetIndexBuffer()->GetSize(), GL_UNSIGNED_INT, 0);
			}
			else {
				glDrawArrays((GLenum)Mesh->GetRenderMode(), 0, Mesh->GetVertexBuffer()[0]->GetSize());
			}
			rend.mesh->Unbind();
		}
	}
}

void Sphynx::Core::Graphics::GL::GLRenderer::Clear()
{
	//TODO:using Nvidia Nsight makes it seem like glClear is Returning GL_Invalid_Operation, there is an error
	//occuring, Fix it.
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
	RenderQueue.clear();
}

void Sphynx::Core::Graphics::GL::GLRenderer::SetDepthTesting(bool value)
{
	if (value)
		glDisable(GL_DEPTH_TEST);
	else
		glEnable(GL_DEPTH_TEST);
}

void Sphynx::Core::Graphics::GL::GLRenderer::SetViewport(Sphynx::Core::Graphics::Viewport view)
{
	glViewport(view.PosX, view.PosY, view.Width, view.Height);
	CurrViewPort = view;
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
		//assume object with the same material will be rendered more then once during execution.
		l = new RenderObjectList();
		RenderQueue[((GLMaterial*)rend.mat)->ProgramId] = l;
	}
	l->push_back(rend);
}
