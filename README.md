# 3D Smooth Blending for subdivision surfaces [C#]
This project is a program that builds and views 3D models, and convert them into subdivision surfaces, smoothing them after each iteration. Subdivision surfaces are initially a control mesh that has been refined through recursive subdivision, each iteration is always defined by the previous iteration's vertices Then the program works on blending two or more objects together while maintaining the smoothness of both objects. Blending is a method for combining freeform objects represented with subdivision surfaces. The technique investigates the deformation of the surfaces and at the same time maintain the smoothness of the blending are that will connect the two objects. <br/>

## 1 - Motivation

The field of computer vision is continuously growing over the years. The use of system based on computer has increased dramatically in the past decade. Film industries use computer vision and blending in animated movies, action movies and others in order to manipulate the objects/environments of the film. Surgeons use systems based on computer vision to display predicted results of surgeries. Numerous mobile applications rely on the developments made in the field of computer vision. Advances in automobile engineering are based on the use of computer vision and many more. Nearly every profession or filed makes use of advances in computer vision. The ongoing growth in use of the field of computer vision is an inspiration to develop the presented software. <br/>

## 2 - Background and Related Work

### 2.1 - What is subdivision? The Fundamentals

Subdivision, is simply defining a smoothed surface as a finite surface resulting from a process that uses a control mesh and repeatedly refining it in multiple levels and constantly adding new vertices, faces and edges.

### 2.2 - Chaikins subdivision algorithm

In Fig. 1, we can see a 4 edge curve (closed) that has been refined 3 times using the corner cutting method, which is a technique used in Chaikins subdivision algorithm. Each control vertex of the curve in the iterative mesh is calculated as a percentage mixture of previous bordering vertices. To that end, the new mesh changes to a smoother curve, which is widely known as uniform quadratic B Spline curve. <br/>

In Chaikins subdivision scheme, the topological rules are shown in Fig. 2, which is widely called as corner cut technique. For each control/previous vertex Vi, we cut off the corner curve by adding two vertices,which create a new edge that connects the two recently added vertices, the overall lengths of all previous edges will be reduced. <br/>

In Chaikins subdivision scheme, the geometric rules are located by the following equation. The recently inserted vertices and they are calculated as a linear combination of old bordering vertices. <br/>

<img src="04_Readme Images/1.png">

### 2.3 - Doo–Sabin subdivision surfaces

Doo-Sabins subdivision surface is considered a general concept of one of Chaikins subdivision’s versions. In a regular rectangular control mesh, uniform biquadratic B Spline surfaces are produced. Here are the set of topological and geometric rules for Doo-Sabin subdivision surfaces. In Fig. 3, we can see the topological rules of Doo–Sabin subdivision surface. For each face that contains n vertices, we will insert n new vertices and are calculated by following the geometric rules that will be discussed later. The new iterative mesh is built by connecting the related vertices to form F-faces, E-faces and V-faces as shown in Fig. 3. Foe each previous face, a new F-face is built using all recently added vertices of the parallel face. For each previous edge, new E-face would also be built by connecting the four recently added vertices of the previous edge. For each previous vertex, a V-face can be built by connecting recently added vertices.

<img src="04_Readme Images/2.png">

### 2.4 - Different subdivision schemes and a brief overview

As mentioned before, any subdivision scheme is known to use a set of geometric and topological rules, these rules are used for mesh improvement in general. Topological rules usually describe how a control mesh is converted to an iterative mesh. The common operations of topological rules may include adding new vertices into faces or edges data structure, renewal of previous vertices, edges, connecting recently inserted/updated vertices (along previous vertices if required), and removing previous unwanted vertices, faces or edges, all these rules are used depending on the style of a subdivision surface we're dealing with. To calculate the exact coordinates of the new iterative vertices we use geometric rules. Some important properties must be considered when trying to design geometric rules for subdivision, finite support with small masks, surface behavior itself, affine invariance, and symmetry. in Fig. 4, some basic topological rules are summarized for subdividing quadrilateral and triangle objects. In some schemes we can find that they incorporate more than one of the topological rules mentioned in Fig. 4. In Fig. 5, a loop subdivision surface is using the one to four splitting topological rule for triangular meshes. Fig. 6 shows a model of a pipe and pistol, made using Catmull-Clarck subdivision and Doo-Sabin subdivision.

<img src="04_Readme Images/3.png">

<img src="04_Readme Images/4.png">

<img src="04_Readme Images/5.png">

### 2.5 - The Half-Edge Representation Structure

What is the Half-Edge Data Structure? It is a common method used to demonstrate a polygon based mesh, using a shared list of points and a list saving polygons saving direct pointers for its vertices. This description is not only appropriate in most situations but also reaches maximum productivity; however, in some other purposes it may be ineffectual. The process of mesh refinement sometimes needs breaking down an edge to a single vertex, which will require removing all the faces neighboring the edge as well as updating the faces that share these vertices. Which demands us to detect adjacency relationships between faces and vertices of the mesh. These operations can most certainly be performed on the simple mesh exemplification mentioned before, but it will definitely be of high cost, many of these operations will need a long search over the whole list of vertices or faces, sometimes both will be needed.
Some adjacency queries on a simple polygon mesh may have:
1) What are the faces that contain this vertex?
2) What are the edges that contain this vertex?
3) What are the faces that neighbor this edge?
4) What are the edges that neighbor this face?
5) What are the faces that are neighbors to this face?

In order to perform these kinds of adjacency queries in an efficient way, the (b-reps) representation method have been developed which is a more sophisticated way to directly model the vertices, faces and edges of the mesh with extra information saved inside. The winged-edge data structure is one of the most popular types of representations, in which edges are enhanced with pointers to the vertices creating that edge, and the two faces neighboring that edge, and also four pointers to the edges which come out from the end points. With this representation structure, we can locate which faces or vertices neighbor a specific edge in fixed time, but, some other kinds of queries may need more costly processing time. 

The half-edge representation is built to be a very advanced version of b-rep, that grant all the queries mentioned before to be performed in constant time. Furthermore, all adjacency information stored inside the faces, edges and vertices stay with fixed space, which means no changing arrays will be used while also providing reasonable compatibility.

With such characteristics that the half-edge data structure carry, it has been an outstanding option for many programs, however it's limited to representing only manifold surfaces, which in some extreme cases can be difficult or impossible to implement.

From the name, the half-edge representation is named this way simply because we don’t need to save the whole edges of a mesh, we only save half-edges, when we split an edge midway, it created two half edges, each called a half of an edge, the two half edged together are called a pair. All half-edges must be directional, and each pair of edges is facing a different direction. Fig.7 illustrates a part of a representation in a triangle mesh using half-edge data structure. Yellow dots represent vertices of the object, blue lines are half-edges. while yellow arrows demonstrate pointers.

<img src="04_Readme Images/6.png">

In Fig.7, all half-edges that neighbor a particular face must form a linked list in a shape of a circle around the face's perimeter. This circle will be directed either clockwise or counter-clockwise as long as it’s around a face, and as long as the same orientations will be used all along. Each of these half-edges included within the circle stores one pointer to the next border, the vertex at the end, and a pointer to its pair.

Each vertex stored within the half-edge representation saves an x, y and z position value, in addition to a pointer pointing to one of the half-edges only, which then may use this vertex as it's initial starting point in order to create the circular linked list we mentioned before. At a given vertex, we will always have more than one half-edge to choose form, however only one is needed and it would not matter which one is chosen. For a very basic form of a half edge representation, each face will need to save a pointer to only one half edge neighboring to it, while being in a more complex environment we would most likely save information on normal vectors, colors and textures as well. Also In the faces, any half edge pointer in the face is very identical to a vertex structure, however in a face there are several half-edges neighboring each of the faces, but in our case we can only need one of them to save and it wouldn't matter which one we choose.

### 2.6 - Libraries Available

2.6.1 OpenGL: Open Graphics Library is an API normally used to associate with GPU, and it has been used for rendering 2D and 3D designs and meshes.<br/>

2.6.2 DirectX: Direct3D (the 3D designs API inside DirectX) is broadly utilized as a part of the improvement of computer games for Microsoft Windows and the Xbox line of consoles. Direct3D is additionally utilized by other programming applications for perception and illustrations undertakings, for example, CAD/CAM designing. As Direct3D is the most broadly plugged part of DirectX, it is regular to see the names "DirectX" and "Direct3D" utilized conversely.


## 3 - Implementation

<img src="04_Readme Images/8.png">

Method Overview

There are six stages of the proposed smooth blending technique for subdivision surfaces. They are the recursive process of sub-dividing the meshes using Catmull-Clark scheme, locating the blend region and removing it, the mesh refinement and boundary smoothing, the creation of vertex correspondence through matching vertices , and connecting the base meshes. In the first stage, a selected mesh will be sub-divided into a finer mesh of control-points that contains more polygon faces. This mesh can then be passed again through the same process. In the second stage, a blend region between two meshes is located with the aid of detecting intersecting faces at a user defined subdivision level for each of the intersecting meshes.

<img src="04_Readme Images/9.png">

Sphere-trees are used for approximating the objects and locating the intersection region. This will eliminate the requirement of calculating the intersecting curve, and thus eliminates the feasible numerical mistakes when the two objects touch. Fig. 13 (b) shows the blend region among two intersecting spheres. In the third stage, Polygons lying in the blend region are discarded as shown in Fig. 13 (c). In the fourth stage, the boundary curve where the blend region ends on both meshes is located, then the boundary points are smoothed out for a smooth blending effect using face normal combined with the original vertices positions. The fifth stage is the creation of vertex correspondence through matching vertices, preparing for stage six where the vertices will be connected through the blend curve.

Class Diagram: 

<img src="04_Readme Images/7.png">

### 3.1 - Sub-division using Catmull–Clark surface

There are three basic steps to subdividing a mesh of control-points:
1- Add a new point to every face, and a new point to every edge. Called the facepoint and the edge point respectively.<br/>
2- Reallocate the controlling point to another position, referred to as the vertexpoint.<br/>
3- Connect the newly inserted points and the reallocated vertex point. Adding new faces to the mesh.<br/>
Stage Input: A mesh of control points<br/>
Stage Output: A finer mesh of control-points that contains more polygon faces.<br/>
This mesh can then be passed again through the same process.

<img src="04_Readme Images/10.png">

As shown, the process can be done multiple times to produce finer meshes.

### 3.2 - Locating the blend region

In this stage, the blend region where the two subdivided meshes may intersect must be located. This is to locate the intersecting faces between the two base meshes. 
Stage Input: two intersected meshes<br/>
Stage Output: Locating the intersected polygons of the two intersected meshes.<br/>

<img src="04_Readme Images/11.png">

<img src="04_Readme Images/12.png">

### 3.3 - Removing the blend region

In this stage, and after the collection of intersected polygons between the two meshes have been determined, the blend region will be discarded.
Stage Input: List of intersected polygons between two meshes.<br/>
Stage Output: Discarding these polygons, determining a list of boundary points and edges.<br/>

<img src="04_Readme Images/12.png">

<img src="04_Readme Images/13.png">

### 3.4 - Boundary smoothing

After discarding the blend region polygons, the resulting boundary curves may have sharp vertices, these sharp corners located on the boundary of the blend region will eventually become sharp corners on the boundary of the blend area, which will eventually lead to a surface with waves. In this stage, the process of smoothing the boundary points is to eliminate these sharp corners and it is done recursively.
Stage Input: List of Boundary points and Boundary edges.<br/>
Stage Output: Smoothed out Boundaries along both meshes.<br/>

<img src="04_Readme Images/14.png">

<img src="04_Readme Images/16.png">

### 3.5 - Matching vertices

After smoothing the boundaries of the two subdivision surfaces, the simplest approach to connecting the base meshes is to connect corresponding points on the two boundaries instantly. However, extraordinary vertices might exist near the boundary curves. In this stage, the controlling points of the meshes along the boundary curves are to be paired together in preparation for connecting them to a new constructed blend curve lying inside the blend region as shown.

Stage Input: List of Boundary points and Boundary edges (After smoothing).<br/>
Stage Output: Vertex pairs.<br/>

<img src="04_Readme Images/15.png">

<img src="04_Readme Images/17.png">


### 3.6 - Connecting base meshes

After successfully pairing the boundary points of both meshes, a blend curve must be established for connecting the base meshes together and generating the blended mesh. The blend curve will be located to keep the extraordinary vertices lying on a flat region of the surface and reduce distortion if possible.

Stage Input: Two lists of the paired vertices.<br/>
Stage Output: Complete blending of the two intersected meshes.<br/>

<img src="04_Readme Images/18.png">
<img src="04_Readme Images/19.png">

## 4 - Complete case study

<img src="04_Readme Images/20.png">
<img src="04_Readme Images/21.png">


## 5 - Conclusion and future work

### 5.1 - Final thoughts

To conclude, the smooth blending for subdivision surfaces as a method is a great tool for modeling complex freeform meshes while maintaining the smooth effect in the blending process. Other blending techniques use vertices on the intersecting curve as control points, which may result in unwanted distortion. That is due to the nature of the intersecting curve having sharp and extraordinary vertices, which in return generate irregular shapes in the region and cause unwanted distortion. The proposed algorithm on the other hand is very reliable, applicable and smooth. Working on this project has benefited me greatly in understanding different types of complex data structures, as well as the ability to understand and implement complex mathematical equations in 3D environment.

### 5.2 - Challenges 

1- Debugging challenges
Due to the nature of the project, various debugging errors have risen up in the implementation process, some of these errors/problems were easy to locate and solve, others were that much harder since we are dealing with hundreds if not thousands of points, edges and faces.

2- Implementing the data structure for this project has been the hardest part, since the data structure is so huge due to the nature of the subdivision schemes, implementing it myself was definitely a challenge. Adding new vertices, edges or faces while constantly updating all the objects, creating new meshes from subdivision scheme using Catmull-Clark surface, removing some polygons while updating objects.

### 5.3 - Future Work

The field of 3D graphics in general and CAD in particular is a very wide and in constant development, so to keep up with the improvement one would have to implement various types of algorithms, techniques and methodologies, such as manipulation tools, which are widely used in CAD programs to manipulate, edit and develop meshes. Some examples of these tools are trimming, Boolean operations, off-setting, intersection and many more. As well as diving into 3D Morphing and deformation.